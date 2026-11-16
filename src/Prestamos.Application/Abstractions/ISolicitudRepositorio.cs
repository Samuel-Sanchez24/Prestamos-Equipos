using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;

namespace Prestamos.Application.Abstractions;

public interface ISolicitudRepositorio
{
    void Agregar(Solicitud solicitud);
    Task<Solicitud?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<List<Solicitud>> ListarAsync(int? solicitanteId, EstadoSolicitud? estado, bool soloVencidas,
        DateOnly? desde, DateOnly? hasta, DateOnly hoy, CancellationToken ct = default);
}