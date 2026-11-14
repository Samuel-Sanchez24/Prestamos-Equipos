namespace Prestamos.Web.Models;

// TEMPORAL: reemplazar por los DTOs de Prestamos.Contracts cuando Samuel los suba.
// La forma de LoginRespuesta es una suposición (no está definida en diseno.md).

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Rol { get; set; } = "";
    public string? TipoSolicitante { get; set; }
}

public class LoginSolicitud
{
    public string Correo { get; set; } = "";
    public string Contrasena { get; set; } = "";
}

public class RegistroSolicitud
{
    public string Nombre { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Correo { get; set; } = "";
    public string TipoSolicitante { get; set; } = "";
    public string Contrasena { get; set; } = "";
}

public class LoginRespuesta
{
    public string Token { get; set; } = "";
    public UsuarioDto? Usuario { get; set; }
}