using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Prestamos.Api.Extensions;

public class EsquemaBearerOpenApi : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken ct)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Pega el token obtenido en POST /api/auth/login"
            }
        };

        foreach (var operacion in document.Paths.Values.SelectMany(p => p.Operations ?? []))
        {
            operacion.Value.Security ??= [];
            operacion.Value.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        }

        return Task.CompletedTask;
    }
}