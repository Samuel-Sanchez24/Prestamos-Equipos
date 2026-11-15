namespace Prestamos.Domain.Entities;

public class Movimiento
{
    public int Id { get; set; }
    public string Entidad { get; set; } = string.Empty;  
    public int EntidadId { get; set; }
    public string? EstadoAnterior { get; set; }
    public string EstadoNuevo { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observacion { get; set; }

    public Usuario Usuario { get; set; } = null!;
}