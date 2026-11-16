using Microsoft.EntityFrameworkCore;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence.Repositories;

public class SolicitudRepositorio : ISolicitudRepositorio
{
    private readonly PrestamosDbContext _db;

    public SolicitudRepositorio(PrestamosDbContext db)
    {
        _db = db;
    }

    public void Agregar(Solicitud solicitud) => _db.Solicitudes.Add(solicitud);

    public Task<Solicitud?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        ConRelaciones().FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<List<Solicitud>> ListarAsync(int? solicitanteId, EstadoSolicitud? estado, bool soloVencidas,
        DateOnly? desde, DateOnly? hasta, DateOnly hoy, CancellationToken ct = default)
    {
        var q = ConRelaciones();

        if (solicitanteId is not null)
            q = q.Where(s => s.SolicitanteId == solicitanteId);
        if (estado is not null)
            q = q.Where(s => s.Estado == estado);
        if (soloVencidas)
            q = q.Where(s => s.Estado == EstadoSolicitud.Entregada && s.FechaFin < hoy
                             && s.Detalles.Any(d => d.FechaDevolucion == null));
        if (desde is not null && hasta is not null)
            q = q.Where(s => s.FechaInicio <= hasta && desde <= s.FechaFin);

        return q.OrderBy(s => s.FechaRegistro).ToListAsync(ct);
    }

    private IQueryable<Solicitud> ConRelaciones() =>
        _db.Solicitudes
           .Include(s => s.Solicitante)
           .Include(s => s.EvaluadaPor)
           .Include(s => s.EntregadaPor)
           .Include(s => s.Detalles).ThenInclude(d => d.Recurso)
           .Include(s => s.Detalles).ThenInclude(d => d.RecibidoPor)
           .Include(s => s.Prorrogas).ThenInclude(p => p.EvaluadaPor)
           .AsSplitQuery();
   
}