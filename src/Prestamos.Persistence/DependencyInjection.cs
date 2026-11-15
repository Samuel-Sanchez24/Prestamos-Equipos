using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PrestamosDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}