namespace Prestamos.Contracts.Usuarios;

public class UsuarioResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? TipoSolicitante { get; set; }
    public bool Activo { get; set; }
}