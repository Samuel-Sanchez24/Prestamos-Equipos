using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Domain.Entities;

public class Prorroga
{
    public int Id { get; set; }
    public int SolicitudId { get; set; }
    public DateOnly FechaFinAnterior { get; set; }
    public DateOnly FechaFinSolicitada { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public EstadoProrroga Estado { get; set; } = EstadoProrroga.Pendiente;
    public DateTime FechaSolicitud { get; set; }

    public int? EvaluadaPorId { get; set; }
    public DateTime? FechaEvaluacion { get; set; }
    public string? MotivoRechazo { get; set; }

    public Solicitud Solicitud { get; set; } = null!;
    public Usuario? EvaluadaPor { get; set; }


    public void Aprobar(int tecnicoId, DateTime ahora)
    {
        ExigirPendiente("aprobar");
        Estado = EstadoProrroga.Aprobada;
        EvaluadaPorId = tecnicoId;
        FechaEvaluacion = ahora;
    }

    public void Rechazar(int tecnicoId, DateTime ahora, string motivo)
    {
        ExigirPendiente("rechazar");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new BusinessRuleException("El motivo del rechazo es obligatorio.");
        Estado = EstadoProrroga.Rechazada;
        EvaluadaPorId = tecnicoId;
        FechaEvaluacion = ahora;
        MotivoRechazo = motivo.Trim();
    }

    private void ExigirPendiente(string accion)
    {
        if (Estado != EstadoProrroga.Pendiente)
            throw new BusinessRuleException($"No se puede {accion} la prórroga {Id} porque está en estado {Estado}.");
    }
}