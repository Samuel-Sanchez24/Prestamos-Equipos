namespace Prestamos.Domain.Entities;

public class DetalleSolicitud
{
    public int Id { get; set; }
    public int SolicitudId { get; set; }
    public int RecursoId { get; set; }

    public DateTime? FechaDevolucion { get; set; }
    public int? RecibidoPorId { get; set; }
    public bool DevueltoConDano { get; set; }
    public string? ObservacionDevolucion { get; set; }

    public Solicitud Solicitud { get; set; } = null!;
    public Recurso Recurso { get; set; } = null!;
    public Usuario? RecibidoPor { get; set; }
}