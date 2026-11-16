using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;

namespace Prestamos.Application.Abstractions;

public interface IProrrogaRepositorio
{
    Task<Prorroga?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<List<Prorroga>> ListarAsync(EstadoProrroga? estado, int? solicitanteId, CancellationToken ct = default);
}