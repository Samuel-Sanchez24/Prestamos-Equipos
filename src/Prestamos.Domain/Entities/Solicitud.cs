using Prestamos.Domain.Enums;

namespace Prestamos.Domain.Entities;

public class Solicitud
{
    public int Id { get; set; }
    public int SolicitanteId { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;
    public DateTime FechaRegistro { get; set; }


    public int? EvaluadaPorId { get; set; }
    public DateTime? FechaEvaluacion { get; set; }
    public string? MotivoRechazo { get; set; }


    public int? EntregadaPorId { get; set; }
    public DateTime? FechaEntrega { get; set; }

    public Usuario Solicitante { get; set; } = null!;
    public Usuario? EvaluadaPor { get; set; }
    public Usuario? EntregadaPor { get; set; }
    public ICollection<DetalleSolicitud> Detalles { get; set; } = new List<DetalleSolicitud>();
    public ICollection<Prorroga> Prorrogas { get; set; } = new List<Prorroga>();
}