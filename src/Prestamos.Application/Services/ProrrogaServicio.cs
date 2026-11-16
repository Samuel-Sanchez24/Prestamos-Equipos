using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Application.Exceptions;
using Prestamos.Application.Options;
using Prestamos.Application.Validators;
using Prestamos.Contracts.Prorrogas;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Application.Services;

public class ProrrogaServicio
{
    private const string EntidadProrroga = "Prorroga";
    private const string EntidadSolicitud = "Solicitud";

    private readonly ISolicitudRepositorio _solicitudes;
    private readonly IProrrogaRepositorio _prorrogas;
    private readonly IRecursoRepositorio _recursos;
    private readonly IMovimientoRepositorio _movimientos;
    private readonly IUnidadDeTrabajo _uow;
    private readonly IReloj _reloj;
    private readonly OpcionesPrestamo _opciones;
    private readonly IValidator<SolicitarProrrogaRequest> _validadorSolicitud;
    private readonly IValidator<RechazarProrrogaRequest> _validadorRechazo;

    public ProrrogaServicio(ISolicitudRepositorio solicitudes, IProrrogaRepositorio prorrogas,
        IRecursoRepositorio recursos, IMovimientoRepositorio movimientos, IUnidadDeTrabajo uow,
        IReloj reloj, OpcionesPrestamo opciones,
        IValidator<SolicitarProrrogaRequest> validadorSolicitud,
        IValidator<RechazarProrrogaRequest> validadorRechazo)
    {
        _solicitudes = solicitudes;
        _prorrogas = prorrogas;
        _recursos = recursos;
        _movimientos = movimientos;
        _uow = uow;
        _reloj = reloj;
        _opciones = opciones;
        _validadorSolicitud = validadorSolicitud;
        _validadorRechazo = validadorRechazo;
    }

    public async Task<ProrrogaResponse> SolicitarAsync(int solicitudId, SolicitarProrrogaRequest req, int usuarioId,
        CancellationToken ct = default)
    {
        await _validadorSolicitud.ValidarOFallarAsync(req, ct);

        var solicitud = await _solicitudes.ObtenerPorIdAsync(solicitudId, ct)
            ?? throw new NoEncontradoException($"No existe la solicitud con id {solicitudId}.");
        if (solicitud.SolicitanteId != usuarioId)
            throw new AccesoDenegadoException("Solo puedes pedir prórroga de tus propios préstamos.");

        ValidarDuracion(solicitud, req.NuevaFechaFin);
        await VerificarCrucesAsync(solicitud, req.NuevaFechaFin, ct);

        var prorroga = solicitud.SolicitarProrroga(req.NuevaFechaFin, req.Motivo, _reloj.Ahora);
        await _uow.GuardarCambiosAsync(ct); // Genera el Id de la prórroga

        _movimientos.Agregar(Movimiento.Registrar(EntidadProrroga, prorroga.Id, null, prorroga.Estado.ToString(),
            usuarioId, _reloj.Ahora, $"Solicita ampliar de {prorroga.FechaFinAnterior:yyyy-MM-dd} a {prorroga.FechaFinSolicitada:yyyy-MM-dd}"));
        await _uow.GuardarCambiosAsync(ct);

        return await RespuestaAsync(prorroga.Id, ct);
    }

    public async Task<List<ProrrogaResponse>> ListarAsync(string? estado, int usuarioId, Rol rol, CancellationToken ct = default)
    {
        estado = estado?.Trim();
        EstadoProrroga? filtro = null;
        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (!Enum.GetNames<EstadoProrroga>().Contains(estado, StringComparer.OrdinalIgnoreCase))
                throw new DatosInvalidosException("estado",
                    $"Estado no válido. Opciones: {string.Join(", ", Enum.GetNames<EstadoProrroga>())}.");
            filtro = Enum.Parse<EstadoProrroga>(estado, ignoreCase: true);
        }

        int? solicitanteId = rol == Rol.Solicitante ? usuarioId : null;
        return (await _prorrogas.ListarAsync(filtro, solicitanteId, ct)).Select(p => p.ARespuesta()).ToList();
    }

    public async Task<ProrrogaResponse> ObtenerAsync(int id, int usuarioId, Rol rol, CancellationToken ct = default)
    {
        var prorroga = await ObtenerOFallarAsync(id, ct);
        if (rol == Rol.Solicitante && prorroga.Solicitud.SolicitanteId != usuarioId)
            throw new AccesoDenegadoException("No puedes consultar prórrogas de otros usuarios.");
        return prorroga.ARespuesta();
    }

    public async Task<ProrrogaResponse> AprobarAsync(int id, int tecnicoId, CancellationToken ct = default)
    {
        await _uow.EnTransaccionAsync(async () =>
        {
            var prorroga = await ObtenerOFallarAsync(id, ct);
            var solicitud = prorroga.Solicitud;
            var finAnterior = solicitud.FechaFin;

            prorroga.Aprobar(tecnicoId, _reloj.Ahora); // Valida que esté pendiente

            var pendientes = RecursosSinDevolver(solicitud);
            if (pendientes.Count > 0)
                await _recursos.BloquearAsync(pendientes, ct);

            ValidarDuracion(solicitud, prorroga.FechaFinSolicitada);
            await VerificarCrucesAsync(solicitud, prorroga.FechaFinSolicitada, ct);

            solicitud.AmpliarFechaFin(prorroga.FechaFinSolicitada);

            _movimientos.Agregar(Movimiento.Registrar(EntidadProrroga, prorroga.Id,
                EstadoProrroga.Pendiente.ToString(), prorroga.Estado.ToString(), tecnicoId, _reloj.Ahora, "Prórroga aprobada"));
            _movimientos.Agregar(Movimiento.Registrar(EntidadSolicitud, solicitud.Id,
                solicitud.Estado.ToString(), solicitud.Estado.ToString(), tecnicoId, _reloj.Ahora,
                $"Fecha de fin ampliada de {finAnterior:yyyy-MM-dd} a {solicitud.FechaFin:yyyy-MM-dd} (prórroga {prorroga.Id})"));

            await _uow.GuardarCambiosAsync(ct);
            return true;
        }, ct);

        return await RespuestaAsync(id, ct);
    }

    public async Task<ProrrogaResponse> RechazarAsync(int id, RechazarProrrogaRequest req, int tecnicoId, CancellationToken ct = default)
    {
        await _validadorRechazo.ValidarOFallarAsync(req, ct);

        var prorroga = await ObtenerOFallarAsync(id, ct);
        prorroga.Rechazar(tecnicoId, _reloj.Ahora, req.Motivo);

        _movimientos.Agregar(Movimiento.Registrar(EntidadProrroga, prorroga.Id,
            EstadoProrroga.Pendiente.ToString(), prorroga.Estado.ToString(), tecnicoId, _reloj.Ahora, req.Motivo.Trim()));
        await _uow.GuardarCambiosAsync(ct);

        return prorroga.ARespuesta();
    }


    private void ValidarDuracion(Solicitud solicitud, DateOnly nuevaFechaFin)
    {
        var tipo = solicitud.Solicitante.TipoSolicitante;
        var maximo = tipo == TipoSolicitante.Docente ? _opciones.DiasMaximoDocente : _opciones.DiasMaximoEstudiante;
        var dias = nuevaFechaFin.DayNumber - solicitud.FechaInicio.DayNumber;
        if (dias > maximo)
            throw new DatosInvalidosException("nuevaFechaFin",
                $"Con la prórroga el préstamo duraría {dias} días. El máximo para {tipo} es de {maximo} días desde la fecha de inicio ({solicitud.FechaInicio:yyyy-MM-dd}).");
    }

    private async Task VerificarCrucesAsync(Solicitud solicitud, DateOnly nuevaFechaFin, CancellationToken ct)
    {
        var pendientes = RecursosSinDevolver(solicitud);
        if (pendientes.Count == 0 || nuevaFechaFin <= solicitud.FechaFin) return;

        var ocupados = await _recursos.ObtenerOcupadosAsync(solicitud.FechaFin.AddDays(1), nuevaFechaFin,
            pendientes, excluirSolicitudId: solicitud.Id, ct: ct);
        if (ocupados.Count > 0)
        {
            var codigos = solicitud.Detalles.Where(d => ocupados.Contains(d.RecursoId)).Select(d => d.Recurso.Codigo);
            throw new BusinessRuleException(
                $"No se puede ampliar hasta {nuevaFechaFin:yyyy-MM-dd}: {string.Join(", ", codigos)} ya está reservado por otra solicitud en ese período.");
        }
    }

    private static List<int> RecursosSinDevolver(Solicitud s) =>
        s.Detalles.Where(d => d.FechaDevolucion is null).Select(d => d.RecursoId).ToList();

    private async Task<Prorroga> ObtenerOFallarAsync(int id, CancellationToken ct) =>
        await _prorrogas.ObtenerPorIdAsync(id, ct)
        ?? throw new NoEncontradoException($"No existe la prórroga con id {id}.");

    private async Task<ProrrogaResponse> RespuestaAsync(int id, CancellationToken ct) =>
        (await ObtenerOFallarAsync(id, ct)).ARespuesta();
}