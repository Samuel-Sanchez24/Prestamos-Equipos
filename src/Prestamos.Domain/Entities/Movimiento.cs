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

    public static Movimiento Registrar(string entidad, int entidadId, string? estadoAnterior,
                                   string estadoNuevo, int usuarioId, DateTime fecha, string? observacion = null) =>
    new()
    {
        Entidad = entidad,
        EntidadId = entidadId,
        EstadoAnterior = estadoAnterior,
        EstadoNuevo = estadoNuevo,
        UsuarioId = usuarioId,
        Fecha = fecha,
        Observacion = observacion
    };
}