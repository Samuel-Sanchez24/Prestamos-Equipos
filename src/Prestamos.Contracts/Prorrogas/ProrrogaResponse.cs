namespace Prestamos.Contracts.Prorrogas;

public class ProrrogaResponse
{
    public int Id { get; set; }
    public int SolicitudId { get; set; }
    public string Solicitante { get; set; } = string.Empty;
    public List<string> Recursos { get; set; } = [];
    public DateOnly FechaFinAnterior { get; set; }
    public DateOnly FechaFinSolicitada { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public string? EvaluadaPor { get; set; }
    public DateTime? FechaEvaluacion { get; set; }
    public string? MotivoRechazo { get; set; }
}