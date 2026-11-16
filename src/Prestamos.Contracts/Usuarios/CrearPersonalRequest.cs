namespace Prestamos.Contracts.Usuarios;

public class CrearPersonalRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // "Tecnico" o "Coordinacion"
    public string Contrasena { get; set; } = string.Empty;
}