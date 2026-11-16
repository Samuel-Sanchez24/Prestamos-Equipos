using Prestamos.Contracts.Usuarios;
using Prestamos.Domain.Entities;

namespace Prestamos.Application.Services;

public static class MapeoUsuario
{
    public static UsuarioResponse ARespuesta(this Usuario u) => new()
    {
        Id = u.Id,
        Nombre = u.Nombre,
        Documento = u.Documento,
        Correo = u.Correo,
        Rol = u.Rol.ToString(),
        TipoSolicitante = u.TipoSolicitante?.ToString(),
        Activo = u.Activo
    };
}