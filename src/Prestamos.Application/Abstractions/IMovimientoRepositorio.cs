using Prestamos.Domain.Entities;

namespace Prestamos.Application.Abstractions;

public interface IMovimientoRepositorio
{
    void Agregar(Movimiento movimiento);
    Task<List<Movimiento>> ListarAsync(string entidad, int entidadId, CancellationToken ct = default);
}