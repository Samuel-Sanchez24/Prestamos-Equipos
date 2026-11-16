using System.Security.Claims;
using Prestamos.Application.Exceptions;

namespace Prestamos.Api.Extensions;

public static class ClaimsExtensions
{
    public static int ObtenerUsuarioId(this ClaimsPrincipal usuario) =>
        int.TryParse(usuario.FindFirstValue("sub"), out var id)
            ? id
            : throw new NoAutenticadoException("La sesión no es válida.");
}