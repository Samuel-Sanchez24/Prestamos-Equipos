using Prestamos.Application.Exceptions;
using Prestamos.Domain.Enums;
using System.Security.Claims;

namespace Prestamos.Api.Extensions;

public static class ClaimsExtensions
{
    public static int ObtenerUsuarioId(this ClaimsPrincipal usuario) =>
        int.TryParse(usuario.FindFirstValue("sub"), out var id)
            ? id
            : throw new NoAutenticadoException("La sesión no es válida.");

    public static Rol ObtenerRol(this ClaimsPrincipal usuario) =>
    Enum.TryParse<Rol>(usuario.FindFirstValue("role"), out var rol)
        ? rol
        : throw new NoAutenticadoException("La sesión no es válida.");
}