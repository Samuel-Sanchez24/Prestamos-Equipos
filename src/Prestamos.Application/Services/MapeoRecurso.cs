using Prestamos.Contracts.Recursos;
using Prestamos.Contracts.Reportes;
using Prestamos.Domain.Entities;

namespace Prestamos.Application.Services;

public static class MapeoRecurso
{
    public static CategoriaResponse ARespuesta(this Categoria c) => new() { Id = c.Id, Nombre = c.Nombre };

    public static RecursoResponse ARespuesta(this Recurso r, bool disponible) => new()
    {
        Id = r.Id,
        Codigo = r.Codigo,
        Nombre = r.Nombre,
        CategoriaId = r.CategoriaId,
        Categoria = r.Categoria?.Nombre ?? string.Empty,
        Descripcion = r.Descripcion,
        Ubicacion = r.Ubicacion,
        EstadoOperativo = r.EstadoOperativo.ToString(),
        DisponibleEnRango = disponible
    };

    public static ReservaResumen AReserva(this Solicitud s) => new()
    {
        SolicitudId = s.Id,
        FechaInicio = s.FechaInicio,
        FechaFin = s.FechaFin,
        Estado = s.Estado.ToString()
    };

    public static MovimientoResponse ARespuesta(this Movimiento m) => new()
    {
        Fecha = m.Fecha,
        EstadoAnterior = m.EstadoAnterior,
        EstadoNuevo = m.EstadoNuevo,
        Usuario = m.Usuario?.Nombre ?? string.Empty,
        Observacion = m.Observacion
    };
}