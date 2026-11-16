namespace Prestamos.Contracts.Recursos;

public class RecursoResponse
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Ubicacion { get; set; }
    public string EstadoOperativo { get; set; } = string.Empty;
    public bool DisponibleEnRango { get; set; }
}