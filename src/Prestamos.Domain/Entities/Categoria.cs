namespace Prestamos.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Recurso> Recursos { get; set; } = new List<Recurso>();
}