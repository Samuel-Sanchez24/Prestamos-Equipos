using Microsoft.EntityFrameworkCore;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence.Repositories;

public class ProrrogaRepositorio : IProrrogaRepositorio
{
    private readonly PrestamosDbContext _db;

    public ProrrogaRepositorio(PrestamosDbContext db)
    {
        _db = db;
    }

    public Task<Prorroga?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        ConRelaciones().FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<List<Prorroga>> ListarAsync(EstadoProrroga? estado, int? solicitanteId, CancellationToken ct = default) =>
        ConRelaciones()
            .Where(p => estado == null || p.Estado == estado)
            .Where(p => solicitanteId == null || p.Solicitud.SolicitanteId == solicitanteId)
            .OrderBy(p => p.FechaSolicitud)
            .ToListAsync(ct);

    private IQueryable<Prorroga> ConRelaciones() =>
        _db.Prorrogas
           .Include(p => p.EvaluadaPor)
           .Include(p => p.Solicitud).ThenInclude(s => s.Solicitante)
           .Include(p => p.Solicitud).ThenInclude(s => s.Detalles).ThenInclude(d => d.Recurso)
           .Include(p => p.Solicitud).ThenInclude(s => s.Prorrogas)
           .AsSplitQuery();
}