using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Prestamos.Web.Models;

namespace Prestamos.Web.Services;

public class ProveedorAutenticacion : AuthenticationStateProvider
{
    UsuarioDto? usuario;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identidad = new ClaimsIdentity(); // sin sesión

        if (usuario != null)
        {
            identidad = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            }, "jwt");
        }

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identidad)));
    }

    public void EstablecerUsuario(UsuarioDto? nuevo)
    {
        usuario = nuevo;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}   