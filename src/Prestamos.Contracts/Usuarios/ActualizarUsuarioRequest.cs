namespace Prestamos.Contracts.Usuarios;

public class ActualizarUsuarioRequest
{
    public string? TipoSolicitante { get; set; }
    public bool? Activo { get; set; }
}