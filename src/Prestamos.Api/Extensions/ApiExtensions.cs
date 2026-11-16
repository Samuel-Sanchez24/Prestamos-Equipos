using Prestamos.Api.Middleware;

namespace Prestamos.Api.Extensions;

public static class ApiExtensions
{
    public const string PoliticaCors = "Web";

    public static IServiceCollection AddApiBase(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddOpenApi(o => o.AddDocumentTransformer<EsquemaBearerOpenApi>());
        services.AddProblemDetails();
        services.AddExceptionHandler<ManejadorExcepciones>();

        var origenes = config.GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? [];
        services.AddCors(o => o.AddPolicy(PoliticaCors, p =>
            p.WithOrigins(origenes).AllowAnyHeader().AllowAnyMethod()));

        return services;
    }

    public static WebApplication UseApiBase(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/openapi/v1.json", "API de Préstamos");
                o.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseCors(PoliticaCors);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}