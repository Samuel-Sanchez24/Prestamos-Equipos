namespace Prestamos.Contracts.Reportes;

public class PrestamoReporteResponse
{
    public int SolicitudId { get; set; }
    public string RecursoCodigo { get; set; } = string.Empty;
    public string RecursoNombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Solicitante { get; set; } = string.Empty;
    public string? TipoSolicitante { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public string? EntregadoPor { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string? RecibidoPor { get; set; }
    public bool DevueltoConDano { get; set; }
    public string Estado { get; set; } = string.Empty; // Activo, Vencido o Devuelto
    public int DiasRetraso { get; set; }
}