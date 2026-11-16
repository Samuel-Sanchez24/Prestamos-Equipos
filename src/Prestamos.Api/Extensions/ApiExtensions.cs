using Prestamos.Api.Middleware;

namespace Prestamos.Api.Extensions;

public static class ApiExtensions
{
    public const string PoliticaCors = "Web";

    public static IServiceCollection AddApiBase(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddOpenApi();

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
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}