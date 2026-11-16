using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prestamos.Persistence.Context;
using Prestamos.Application.Abstractions;
using Prestamos.Persistence.Repositories;

namespace Prestamos.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PrestamosDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

        services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
        services.AddScoped<IRecursoRepositorio, RecursoRepositorio>();
        services.AddScoped<IMovimientoRepositorio, MovimientoRepositorio>();

        services.AddScoped<ISolicitudRepositorio, SolicitudRepositorio>();

        services.AddScoped<IProrrogaRepositorio, ProrrogaRepositorio>();

        services.AddScoped<IReporteRepositorio, ReporteRepositorio>();

        return services;
    }
}