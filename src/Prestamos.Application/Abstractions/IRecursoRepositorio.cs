using Prestamos.Domain.Entities;

namespace Prestamos.Application.Abstractions;

public interface IRecursoRepositorio
{
    Task<List<Recurso>> ListarAsync(int? categoriaId, bool incluirDadosDeBaja, CancellationToken ct = default);
    Task<Recurso?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteCodigoAsync(string codigo, CancellationToken ct = default);
    void Agregar(Recurso recurso);

 
    Task<HashSet<int>> ObtenerOcupadosAsync(DateOnly desde, DateOnly hasta,
        IReadOnlyCollection<int>? recursoIds = null, int? excluirSolicitudId = null, CancellationToken ct = default);

    Task<List<Solicitud>> ObtenerReservasVigentesAsync(int recursoId, DateOnly desde, CancellationToken ct = default);
    Task<List<Recurso>> ObtenerPorIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct = default);

    Task BloquearAsync(IReadOnlyCollection<int> ids, CancellationToken ct = default);
}