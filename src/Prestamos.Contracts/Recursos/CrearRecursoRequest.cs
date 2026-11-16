namespace Prestamos.Contracts.Recursos;

public class CrearRecursoRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string? Descripcion { get; set; }
    public string? Ubicacion { get; set; }
}