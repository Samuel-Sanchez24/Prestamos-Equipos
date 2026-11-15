using Prestamos.Domain.Enums;

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
}