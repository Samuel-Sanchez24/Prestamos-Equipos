using Prestamos.Domain.Entities;

namespace Prestamos.Application.Abstractions;

public interface ICategoriaRepositorio
{
    Task<List<Categoria>> ListarAsync(CancellationToken ct = default);
    Task<bool> ExisteAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(string nombre, CancellationToken ct = default);
    void Agregar(Categoria categoria);
}