using Prestamos.Contracts.Prorrogas;

namespace Prestamos.Contracts.Solicitudes;

public class SolicitudResponse
{
    public int Id { get; set; }
    public int SolicitanteId { get; set; }
    public string Solicitante { get; set; } = string.Empty;
    public string? TipoSolicitante { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Vencida { get; set; }
    public DateTime FechaRegistro { get; set; }

    public string? EvaluadaPor { get; set; }
    public DateTime? FechaEvaluacion { get; set; }
    public string? MotivoRechazo { get; set; }
    public string? EntregadaPor { get; set; }
    public DateTime? FechaEntrega { get; set; }

    public List<DetalleRecursoResponse> Recursos { get; set; } = [];
    public List<ProrrogaResponse> Prorrogas { get; set; } = [];
}