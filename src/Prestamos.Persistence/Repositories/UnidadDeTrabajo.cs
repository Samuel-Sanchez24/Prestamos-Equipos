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

    public async Task<T> EnTransaccionAsync<T>(Func<Task<T>> operacion, CancellationToken ct = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var resultado = await operacion();
            await tx.CommitAsync(ct);
            return resultado;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            _db.ChangeTracker.Clear(); // Descarta lo que quedó a medias
            throw;
        }
    }
}