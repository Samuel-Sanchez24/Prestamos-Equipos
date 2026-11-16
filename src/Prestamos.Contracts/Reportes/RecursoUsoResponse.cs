namespace Prestamos.Contracts.Reportes;

public class RecursoUsoResponse
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int Prestamos { get; set; }
}