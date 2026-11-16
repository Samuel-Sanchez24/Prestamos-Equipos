using Microsoft.EntityFrameworkCore;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence.Repositories;

public class RecursoRepositorio : IRecursoRepositorio
{
    private readonly PrestamosDbContext _db;
    private readonly IReloj _reloj;

    public RecursoRepositorio(PrestamosDbContext db, IReloj reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    public Task<List<Recurso>> ListarAsync(int? categoriaId, bool incluirDadosDeBaja, CancellationToken ct = default) =>
        _db.Recursos
           .Include(r => r.Categoria)
           .Where(r => categoriaId == null || r.CategoriaId == categoriaId)
           .Where(r => incluirDadosDeBaja || r.EstadoOperativo != EstadoRecurso.DadoDeBaja)
           .OrderBy(r => r.Codigo)
           .ToListAsync(ct);

    public Task<Recurso?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Recursos.Include(r => r.Categoria).FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<bool> ExisteCodigoAsync(string codigo, CancellationToken ct = default) =>
        _db.Recursos.AnyAsync(r => r.Codigo == codigo, ct);

    public void Agregar(Recurso recurso) => _db.Recursos.Add(recurso);

    public async Task<HashSet<int>> ObtenerOcupadosAsync(DateOnly desde, DateOnly hasta,
        IReadOnlyCollection<int>? recursoIds = null, int? excluirSolicitudId = null, CancellationToken ct = default)
    {
        var hoy = _reloj.Hoy;

        var consulta = _db.DetallesSolicitud.Where(d =>
            ((d.Solicitud.Estado == EstadoSolicitud.Pendiente || d.Solicitud.Estado == EstadoSolicitud.Aprobada)
                && d.Solicitud.FechaInicio <= hasta && desde <= d.Solicitud.FechaFin)
            ||
            (d.Solicitud.Estado == EstadoSolicitud.Entregada && d.FechaDevolucion == null
                && d.Solicitud.FechaInicio <= hasta
                && (desde <= d.Solicitud.FechaFin || d.Solicitud.FechaFin < hoy)));

        if (recursoIds is not null)
            consulta = consulta.Where(d => recursoIds.Contains(d.RecursoId));
        if (excluirSolicitudId is not null)
            consulta = consulta.Where(d => d.SolicitudId != excluirSolicitudId);

        var ids = await consulta.Select(d => d.RecursoId).Distinct().ToListAsync(ct);
        return ids.ToHashSet();
    }

    public Task<List<Solicitud>> ObtenerReservasVigentesAsync(int recursoId, DateOnly desde, CancellationToken ct = default) =>
        _db.DetallesSolicitud
           .Where(d => d.RecursoId == recursoId)
           .Where(d =>
               ((d.Solicitud.Estado == EstadoSolicitud.Pendiente || d.Solicitud.Estado == EstadoSolicitud.Aprobada)
                   && d.Solicitud.FechaFin >= desde)
               || (d.Solicitud.Estado == EstadoSolicitud.Entregada && d.FechaDevolucion == null))
           .Select(d => d.Solicitud)
           .OrderBy(s => s.FechaInicio)
           .ToListAsync(ct);
}