using Microsoft.EntityFrameworkCore;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence.Repositories;

public class CategoriaRepositorio : ICategoriaRepositorio
{
    private readonly PrestamosDbContext _db;

    public CategoriaRepositorio(PrestamosDbContext db)
    {
        _db = db;
    }

    public Task<List<Categoria>> ListarAsync(CancellationToken ct = default) =>
        _db.Categorias.OrderBy(c => c.Nombre).ToListAsync(ct);

    public Task<bool> ExisteAsync(int id, CancellationToken ct = default) =>
        _db.Categorias.AnyAsync(c => c.Id == id, ct);

    public Task<bool> ExisteNombreAsync(string nombre, CancellationToken ct = default) =>
        _db.Categorias.AnyAsync(c => c.Nombre == nombre, ct);

    public void Agregar(Categoria categoria) => _db.Categorias.Add(categoria);
}