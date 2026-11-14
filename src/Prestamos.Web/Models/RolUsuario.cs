namespace Prestamos.Web.Models;

// Valores exactos que usa la API (sin tildes)
public static class RolUsuario
{
    public const string Solicitante = "Solicitante";
    public const string Tecnico = "Tecnico";
    public const string Coordinacion = "Coordinacion";
    public const string Administrador = "Administrador";

    // Texto para mostrar en pantalla
    public static string Etiqueta(string? rol) => rol switch
    {
        Tecnico => "Técnico",
        Coordinacion => "Coordinación",
        _ => rol ?? ""
    };
}   