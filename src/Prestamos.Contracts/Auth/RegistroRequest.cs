namespace Prestamos.Contracts.Auth;

public class RegistroRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string TipoSolicitante { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
}