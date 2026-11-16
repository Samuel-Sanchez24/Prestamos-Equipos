using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Prestamos.Api.Security;
using Prestamos.Api.Services;
using Prestamos.Application.Abstractions;

namespace Prestamos.Api.Extensions;

public static class SeguridadExtensions
{
    public static IServiceCollection AddSeguridad(this IServiceCollection services, IConfiguration config)
    {
        var seccion = config.GetSection("Jwt");
        var jwt = seccion.Get<OpcionesJwt>() ?? new OpcionesJwt();

        if (Encoding.UTF8.GetByteCount(jwt.Clave) < 32)
            throw new InvalidOperationException(
                "Falta 'Jwt:Clave' o tiene menos de 32 caracteres. Configúrala con los secretos de usuario (ver README).");

        services.Configure<OpcionesJwt>(seccion);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Emisor,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audiencia,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();

        services.AddSingleton<IHashContrasena, HashContrasena>();
        services.AddSingleton<IGeneradorToken, GeneradorTokenJwt>();
        services.AddSingleton<IReloj, RelojColombia>();

        return services;
    }
}