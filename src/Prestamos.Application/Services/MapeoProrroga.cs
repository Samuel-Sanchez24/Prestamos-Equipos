using Prestamos.Contracts.Prorrogas;
using Prestamos.Domain.Entities;

namespace Prestamos.Application.Services;

public static class MapeoProrroga
{
    public static ProrrogaResponse ARespuesta(this Prorroga p) => new()
    {
        Id = p.Id,
        SolicitudId = p.SolicitudId,
        Solicitante = p.Solicitud?.Solicitante?.Nombre ?? string.Empty,
        Recursos = p.Solicitud?.Detalles.Select(d => d.Recurso?.Codigo ?? string.Empty).OrderBy(c => c).ToList() ?? [],
        FechaFinAnterior = p.FechaFinAnterior,
        FechaFinSolicitada = p.FechaFinSolicitada,
        Motivo = p.Motivo,
        Estado = p.Estado.ToString(),
        FechaSolicitud = p.FechaSolicitud,
        EvaluadaPor = p.EvaluadaPor?.Nombre,
        FechaEvaluacion = p.FechaEvaluacion,
        MotivoRechazo = p.MotivoRechazo
    };
}