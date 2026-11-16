namespace Prestamos.Contracts.Solicitudes;

public class DetalleRecursoResponse
{
    public int RecursoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateTime? FechaDevolucion { get; set; }
    public string? RecibidoPor { get; set; }
    public bool DevueltoConDano { get; set; }
    public string? ObservacionDevolucion { get; set; }
}