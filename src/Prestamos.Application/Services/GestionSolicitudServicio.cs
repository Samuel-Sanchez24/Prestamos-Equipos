using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Application.Exceptions;
using Prestamos.Application.Validators;
using Prestamos.Contracts.Solicitudes;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Application.Services;

public class GestionSolicitudServicio
{
    private const string EntidadSolicitud = "Solicitud";
    private const string EntidadRecurso = "Recurso";

    private readonly ISolicitudRepositorio _solicitudes;
    private readonly IRecursoRepositorio _recursos;
    private readonly IMovimientoRepositorio _movimientos;
    private readonly IUnidadDeTrabajo _uow;
    private readonly IReloj _reloj;
    private readonly IValidator<RechazarSolicitudRequest> _validadorRechazo;
    private readonly IValidator<RegistrarDevolucionRequest> _validadorDevolucion;

    public GestionSolicitudServicio(ISolicitudRepositorio solicitudes, IRecursoRepositorio recursos,
        IMovimientoRepositorio movimientos, IUnidadDeTrabajo uow, IReloj reloj,
        IValidator<RechazarSolicitudRequest> validadorRechazo,
        IValidator<RegistrarDevolucionRequest> validadorDevolucion)
    {
        _solicitudes = solicitudes;
        _recursos = recursos;
        _movimientos = movimientos;
        _uow = uow;
        _reloj = reloj;
        _validadorRechazo = validadorRechazo;
        _validadorDevolucion = validadorDevolucion;
    }

    public async Task<SolicitudResponse> AprobarAsync(int id, int tecnicoId, CancellationToken ct = default)
    {
        await _uow.EnTransaccionAsync(async () =>
        {
            var solicitud = await ObtenerOFallarAsync(id, ct);
            var recursoIds = solicitud.Detalles.Select(d => d.RecursoId).ToList();

            await _recursos.BloquearAsync(recursoIds, ct);

            var noOperativos = solicitud.Detalles
                .Where(d => d.Recurso.EstadoOperativo != EstadoRecurso.Disponible)
                .Select(d => $"{d.Recurso.Codigo} ({d.Recurso.EstadoOperativo})")
                .ToList();
            if (noOperativos.Count > 0)
                throw new BusinessRuleException(
                    $"No se puede aprobar: {string.Join(", ", noOperativos)}. Rechaza la solicitud indicando el motivo.");

            var ocupados = await _recursos.ObtenerOcupadosAsync(solicitud.FechaInicio, solicitud.FechaFin,
                recursoIds, excluirSolicitudId: solicitud.Id, ct: ct);
            if (ocupados.Count > 0)
                throw new BusinessRuleException("Uno o más recursos ya tienen otra reserva en esas fechas.");

            var anterior = solicitud.Estado;
            solicitud.Aprobar(tecnicoId, _reloj.Ahora);
            RegistrarMovimiento(solicitud, anterior, tecnicoId, "Solicitud aprobada");
            await _uow.GuardarCambiosAsync(ct);
            return true;
        }, ct);

        return await RespuestaAsync(id, ct);
    }

    public async Task<SolicitudResponse> RechazarAsync(int id, RechazarSolicitudRequest req, int tecnicoId, CancellationToken ct = default)
    {
        await _validadorRechazo.ValidarOFallarAsync(req, ct);

        var solicitud = await ObtenerOFallarAsync(id, ct);
        var anterior = solicitud.Estado;
        solicitud.Rechazar(tecnicoId, _reloj.Ahora, req.Motivo);
        RegistrarMovimiento(solicitud, anterior, tecnicoId, req.Motivo.Trim());
        await _uow.GuardarCambiosAsync(ct);

        return solicitud.ARespuesta(_reloj.Hoy);
    }

    public async Task<SolicitudResponse> EntregarAsync(int id, int tecnicoId, CancellationToken ct = default)
    {
        var solicitud = await ObtenerOFallarAsync(id, ct);

        var noOperativos = solicitud.Detalles
            .Where(d => d.Recurso.EstadoOperativo != EstadoRecurso.Disponible)
            .Select(d => $"{d.Recurso.Codigo} ({d.Recurso.EstadoOperativo})")
            .ToList();
        if (noOperativos.Count > 0)
            throw new BusinessRuleException(
                $"No se puede entregar: {string.Join(", ", noOperativos)}. Márcala como no recogida o pide al solicitante cancelarla.");

        var anterior = solicitud.Estado;
        solicitud.Entregar(tecnicoId, _reloj.Ahora, _reloj.Hoy);
        RegistrarMovimiento(solicitud, anterior, tecnicoId, "Recursos entregados");
        await _uow.GuardarCambiosAsync(ct);

        return solicitud.ARespuesta(_reloj.Hoy);
    }

    public async Task<SolicitudResponse> MarcarNoRecogidaAsync(int id, int tecnicoId, CancellationToken ct = default)
    {
        var solicitud = await ObtenerOFallarAsync(id, ct);
        var anterior = solicitud.Estado;
        solicitud.MarcarNoRecogida();
        RegistrarMovimiento(solicitud, anterior, tecnicoId, "El solicitante no recogió los recursos");
        await _uow.GuardarCambiosAsync(ct);

        return solicitud.ARespuesta(_reloj.Hoy);
    }

    public async Task<SolicitudResponse> RegistrarDevolucionAsync(int id, RegistrarDevolucionRequest req, int tecnicoId, CancellationToken ct = default)
    {
        await _validadorDevolucion.ValidarOFallarAsync(req, ct);

        var solicitud = await ObtenerOFallarAsync(id, ct);
        var anterior = solicitud.Estado;
        var ahora = _reloj.Ahora;

        var detalle = solicitud.RegistrarDevolucion(req.RecursoId, tecnicoId, ahora, req.DevueltoConDano, req.Observacion);
        var recurso = detalle.Recurso;

        var nota = $"Devuelto {recurso.Codigo}" + (req.DevueltoConDano ? " con daño" : "") +
                   (string.IsNullOrWhiteSpace(req.Observacion) ? "" : $": {req.Observacion.Trim()}");
        RegistrarMovimiento(solicitud, anterior, tecnicoId, nota);

        if (req.DevueltoConDano && recurso.EstadoOperativo == EstadoRecurso.Disponible)
        {
            var estadoRecurso = recurso.EstadoOperativo;
            recurso.MarcarDanado();
            _movimientos.Agregar(Movimiento.Registrar(EntidadRecurso, recurso.Id, estadoRecurso.ToString(),
                recurso.EstadoOperativo.ToString(), tecnicoId, ahora, $"Devuelto con daño en la solicitud {solicitud.Id}"));
        }

        await _uow.GuardarCambiosAsync(ct);
        return solicitud.ARespuesta(_reloj.Hoy);
    }

    private void RegistrarMovimiento(Solicitud solicitud, EstadoSolicitud anterior, int usuarioId, string? observacion) =>
        _movimientos.Agregar(Movimiento.Registrar(EntidadSolicitud, solicitud.Id, anterior.ToString(),
            solicitud.Estado.ToString(), usuarioId, _reloj.Ahora, observacion));

    private async Task<Solicitud> ObtenerOFallarAsync(int id, CancellationToken ct) =>
        await _solicitudes.ObtenerPorIdAsync(id, ct)
        ?? throw new NoEncontradoException($"No existe la solicitud con id {id}.");

    private async Task<SolicitudResponse> RespuestaAsync(int id, CancellationToken ct) =>
        (await ObtenerOFallarAsync(id, ct)).ARespuesta(_reloj.Hoy);
}