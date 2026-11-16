using Prestamos.Domain.Entities;

namespace Prestamos.Application.Abstractions;

public interface IReporteRepositorio
{
    Task<List<Solicitud>> ListarSolicitudesAsync(int? categoriaId, CancellationToken ct = default);
}