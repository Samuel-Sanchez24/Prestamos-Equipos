using Prestamos.Contracts.Solicitudes;
using Prestamos.Domain.Entities;

namespace Prestamos.Application.Services;

public static class MapeoSolicitud
{
    public static SolicitudResponse ARespuesta(this Solicitud s, DateOnly hoy) => new()
    {
        Id = s.Id,
        SolicitanteId = s.SolicitanteId,
        Solicitante = s.Solicitante?.Nombre ?? string.Empty,
        TipoSolicitante = s.Solicitante?.TipoSolicitante?.ToString(),
        FechaInicio = s.FechaInicio,
        FechaFin = s.FechaFin,
        Motivo = s.Motivo,
        Estado = s.Estado.ToString(),
        Vencida = s.EstaVencida(hoy),
        FechaRegistro = s.FechaRegistro,
        EvaluadaPor = s.EvaluadaPor?.Nombre,
        FechaEvaluacion = s.FechaEvaluacion,
        MotivoRechazo = s.MotivoRechazo,
        EntregadaPor = s.EntregadaPor?.Nombre,
        FechaEntrega = s.FechaEntrega,
        Recursos = s.Detalles.OrderBy(d => d.Recurso?.Codigo).Select(d => new DetalleRecursoResponse
        {
            RecursoId = d.RecursoId,
            Codigo = d.Recurso?.Codigo ?? string.Empty,
            Nombre = d.Recurso?.Nombre ?? string.Empty,
            FechaDevolucion = d.FechaDevolucion,
            RecibidoPor = d.RecibidoPor?.Nombre,
            DevueltoConDano = d.DevueltoConDano,
            ObservacionDevolucion = d.ObservacionDevolucion
        }).ToList()
    };
}