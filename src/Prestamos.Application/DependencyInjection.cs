using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Prestamos.Application.Services;

namespace Prestamos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<AuthServicio>();
        services.AddScoped<UsuarioServicio>();

        services.AddScoped<CategoriaServicio>();
        services.AddScoped<RecursoServicio>();

        services.AddScoped<SolicitudServicio>();

        services.AddScoped<GestionSolicitudServicio>();

        services.AddScoped<ProrrogaServicio>();

        services.AddScoped<ReporteServicio>();
        return services;
    }
}