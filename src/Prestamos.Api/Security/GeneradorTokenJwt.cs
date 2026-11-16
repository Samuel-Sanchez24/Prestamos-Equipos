using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;

namespace Prestamos.Api.Security;

public class GeneradorTokenJwt : IGeneradorToken
{
    private readonly OpcionesJwt _opciones;

    public GeneradorTokenJwt(IOptions<OpcionesJwt> opciones)
    {
        _opciones = opciones.Value;
    }

    public (string Token, DateTime ExpiraEn) Generar(Usuario usuario)
    {
        var expira = DateTime.UtcNow.AddMinutes(_opciones.MinutosExpiracion);

        var claims = new List<Claim>
        {
            new("sub", usuario.Id.ToString()),
            new("name", usuario.Nombre),
            new("email", usuario.Correo),
            new("role", usuario.Rol.ToString())
        };
        if (usuario.TipoSolicitante is not null)
            claims.Add(new("tipo", usuario.TipoSolicitante.Value.ToString()));

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.Clave));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _opciones.Emisor,
            Audience = _opciones.Audiencia,
            Expires = expira,
            SigningCredentials = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return (token, expira);
    }
}