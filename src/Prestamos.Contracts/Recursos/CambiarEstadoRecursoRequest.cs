namespace Prestamos.Contracts.Recursos;

public class CambiarEstadoRecursoRequest
{
    public string Estado { get; set; } = string.Empty; // "Danado", "Disponible" o "DadoDeBaja"
    public string? Observacion { get; set; }
}