using Microsoft.EntityFrameworkCore;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence.Repositories;

public class MovimientoRepositorio : IMovimientoRepositorio
{
    private readonly PrestamosDbContext _db;

    public MovimientoRepositorio(PrestamosDbContext db)
    {
        _db = db;
    }

    public void Agregar(Movimiento movimiento) => _db.Movimientos.Add(movimiento);

    public Task<List<Movimiento>> ListarAsync(string entidad, int entidadId, CancellationToken ct = default) =>
        _db.Movimientos
           .Include(m => m.Usuario)
           .Where(m => m.Entidad == entidad && m.EntidadId == entidadId)
           .OrderBy(m => m.Fecha)
           .ToListAsync(ct);
}