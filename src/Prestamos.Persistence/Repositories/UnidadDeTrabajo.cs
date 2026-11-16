using Prestamos.Application.Abstractions;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence.Repositories;

public class UnidadDeTrabajo : IUnidadDeTrabajo
{
    private readonly PrestamosDbContext _db;

    public UnidadDeTrabajo(PrestamosDbContext db)
    {
        _db = db;
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}