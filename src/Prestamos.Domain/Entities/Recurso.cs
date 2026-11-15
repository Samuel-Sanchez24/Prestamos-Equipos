using Prestamos.Domain.Enums;

namespace Prestamos.Domain.Entities;

public class Recurso
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string? Descripcion { get; set; }
    public string? Ubicacion { get; set; }
    public EstadoRecurso EstadoOperativo { get; set; } = EstadoRecurso.Disponible;
    public DateTime FechaRegistro { get; set; }

    public Categoria Categoria { get; set; } = null!;
    public ICollection<DetalleSolicitud> Detalles { get; set; } = new List<DetalleSolicitud>();
}