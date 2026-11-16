namespace Prestamos.Contracts.Recursos;

public class ActualizarRecursoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string? Descripcion { get; set; }
    public string? Ubicacion { get; set; }
}