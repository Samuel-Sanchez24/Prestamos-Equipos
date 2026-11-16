using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Application.Exceptions;
using Prestamos.Application.Options;
using Prestamos.Application.Validators;
using Prestamos.Contracts.Reportes;
using Prestamos.Contracts.Solicitudes;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Application.Services;

public class SolicitudServicio
{
    private const string Entidad = "Solicitud";

    private readonly ISolicitudRepositorio _solicitudes;
    private readonly IRecursoRepositorio _recursos;
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IMovimientoRepositorio _movimientos;
    private readonly IUnidadDeTrabajo _uow;
    private readonly IReloj _reloj;
    private readonly OpcionesPrestamo _opciones;
    private readonly IValidator<CrearSolicitudRequest> _validador;

    public SolicitudServicio(ISolicitudRepositorio solicitudes, IRecursoRepositorio recursos,
        IUsuarioRepositorio usuarios, IMovimientoRepositorio movimientos, IUnidadDeTrabajo uow,
        IReloj reloj, OpcionesPrestamo opciones, IValidator<CrearSolicitudRequest> validador)
    {
        _solicitudes = solicitudes;
        _recursos = recursos;
        _usuarios = usuarios;
        _movimientos = movimientos;
        _uow = uow;
        _reloj = reloj;
        _opciones = opciones;
        _validador = validador;
    }

    public async Task<SolicitudResponse> CrearAsync(CrearSolicitudRequest req, int usuarioId, CancellationToken ct = default)
    {
        await _validador.ValidarOFallarAsync(req, ct);

        var solicitante = await _usuarios.ObtenerPorIdAsync(usuarioId, ct)
            ?? throw new NoAutenticadoException("La sesión no es válida.");
        if (solicitante.Rol != Rol.Solicitante || solicitante.TipoSolicitante is null)
            throw new AccesoDenegadoException("Solo los estudiantes y docentes pueden registrar solicitudes.");

        var dias = req.FechaFin.DayNumber - req.FechaInicio.DayNumber;
        var maximo = solicitante.TipoSolicitante == TipoSolicitante.Docente
            ? _opciones.DiasMaximoDocente
            : _opciones.DiasMaximoEstudiante;
        if (dias > maximo)
            throw new DatosInvalidosException("fechaFin",
                $"La duración máxima para {solicitante.TipoSolicitante} es de {maximo} días. Solicitaste {dias}.");

        var recursos = await _recursos.ObtenerPorIdsAsync(req.RecursoIds, ct);
        var faltantes = req.RecursoIds.Except(recursos.Select(r => r.Id)).ToList();
        if (faltantes.Count > 0)
            throw new NoEncontradoException($"No existen los recursos con id: {string.Join(", ", faltantes)}.");

        var id = await _uow.EnTransaccionAsync(async () =>
        {
            await _recursos.BloquearAsync(req.RecursoIds, ct);

            var actuales = await _recursos.ObtenerPorIdsAsync(req.RecursoIds, ct);
            var noOperativos = actuales.Where(r => r.EstadoOperativo != EstadoRecurso.Disponible).ToList();
            if (noOperativos.Count > 0)
                throw new BusinessRuleException(
                    $"No se pueden reservar: {string.Join(", ", noOperativos.Select(r => $"{r.Codigo} ({r.EstadoOperativo})"))}.");

            var ocupados = await _recursos.ObtenerOcupadosAsync(req.FechaInicio, req.FechaFin, req.RecursoIds, ct: ct);
            if (ocupados.Count > 0)
            {
                var codigos = actuales.Where(r => ocupados.Contains(r.Id)).Select(r => r.Codigo);
                throw new BusinessRuleException(
                    $"Ya están reservados entre {req.FechaInicio:yyyy-MM-dd} y {req.FechaFin:yyyy-MM-dd}: {string.Join(", ", codigos)}.");
            }

            var solicitud = new Solicitud
            {
                SolicitanteId = usuarioId,
                FechaInicio = req.FechaInicio,
                FechaFin = req.FechaFin,
                Motivo = req.Motivo.Trim(),
                Estado = EstadoSolicitud.Pendiente,
                FechaRegistro = _reloj.Ahora,
                Detalles = req.RecursoIds.Select(rid => new DetalleSolicitud { RecursoId = rid }).ToList()
            };
            _solicitudes.Agregar(solicitud);
            await _uow.GuardarCambiosAsync(ct);

            _movimientos.Agregar(Movimiento.Registrar(Entidad, solicitud.Id, null, solicitud.Estado.ToString(),
                usuarioId, _reloj.Ahora, "Solicitud registrada"));
            await _uow.GuardarCambiosAsync(ct);

            return solicitud.Id;
        }, ct);

        return (await ObtenerOFallarAsync(id, ct)).ARespuesta(_reloj.Hoy);
    }

    public async Task<List<SolicitudResponse>> ListarAsync(string? estado, bool vencidas, int? solicitanteId,
        DateOnly? desde, DateOnly? hasta, int usuarioId, Rol rol, CancellationToken ct = default)
    {
        EstadoSolicitud? filtroEstado = null;
        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (!Enum.GetNames<EstadoSolicitud>().Contains(estado, StringComparer.OrdinalIgnoreCase))
                throw new DatosInvalidosException("estado",
                    $"Estado no válido. Opciones: {string.Join(", ", Enum.GetNames<EstadoSolicitud>())}.");
            filtroEstado = Enum.Parse<EstadoSolicitud>(estado, ignoreCase: true);
        }

        if (desde.HasValue != hasta.HasValue)
            throw new DatosInvalidosException("desde", "Indica 'desde' y 'hasta' juntos, o ninguno.");
        if (desde > hasta)
            throw new DatosInvalidosException("hasta", "La fecha 'hasta' no puede ser anterior a 'desde'.");

        if (rol == Rol.Solicitante)
            solicitanteId = usuarioId;

        var hoy = _reloj.Hoy;
        var lista = await _solicitudes.ListarAsync(solicitanteId, filtroEstado, vencidas, desde, hasta, hoy, ct);
        return lista.Select(s => s.ARespuesta(hoy)).ToList();
    }

    public async Task<SolicitudResponse> ObtenerAsync(int id, int usuarioId, Rol rol, CancellationToken ct = default)
    {
        var solicitud = await ObtenerOFallarAsync(id, ct);
        VerificarAcceso(solicitud, usuarioId, rol);
        return solicitud.ARespuesta(_reloj.Hoy);
    }

    public async Task<SolicitudResponse> CancelarAsync(int id, int usuarioId, CancellationToken ct = default)
    {
        var solicitud = await ObtenerOFallarAsync(id, ct);
        if (solicitud.SolicitanteId != usuarioId)
            throw new AccesoDenegadoException("Solo puedes cancelar tus propias solicitudes.");

        var anterior = solicitud.Estado;
        solicitud.Cancelar();

        _movimientos.Agregar(Movimiento.Registrar(Entidad, solicitud.Id, anterior.ToString(),
            solicitud.Estado.ToString(), usuarioId, _reloj.Ahora, "Cancelada por el solicitante"));
        await _uow.GuardarCambiosAsync(ct);

        return solicitud.ARespuesta(_reloj.Hoy);
    }

    public async Task<List<MovimientoResponse>> ListarMovimientosAsync(int id, int usuarioId, Rol rol, CancellationToken ct = default)
    {
        var solicitud = await ObtenerOFallarAsync(id, ct);
        VerificarAcceso(solicitud, usuarioId, rol);
        return (await _movimientos.ListarAsync(Entidad, id, ct)).Select(m => m.ARespuesta()).ToList();
    }

    private static void VerificarAcceso(Solicitud solicitud, int usuarioId, Rol rol)
    {
        if (rol == Rol.Solicitante && solicitud.SolicitanteId != usuarioId)
            throw new AccesoDenegadoException("No puedes consultar solicitudes de otros usuarios.");
    }

    private async Task<Solicitud> ObtenerOFallarAsync(int id, CancellationToken ct) =>
        await _solicitudes.ObtenerPorIdAsync(id, ct)
        ?? throw new NoEncontradoException($"No existe la solicitud con id {id}.");
}