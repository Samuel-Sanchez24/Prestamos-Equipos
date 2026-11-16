using Prestamos.Application.Abstractions;
using Prestamos.Application.Exceptions;
using Prestamos.Contracts.Reportes;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;

namespace Prestamos.Application.Services;

public class ReporteServicio
{
    private static readonly string[] EstadosPrestamo = ["Activo", "Vencido", "Devuelto"];

    private readonly IReporteRepositorio _reportes;
    private readonly IRecursoRepositorio _recursos;
    private readonly IReloj _reloj;

    public ReporteServicio(IReporteRepositorio reportes, IRecursoRepositorio recursos, IReloj reloj)
    {
        _reportes = reportes;
        _recursos = recursos;
        _reloj = reloj;
    }

    public async Task<ResumenResponse> ObtenerResumenAsync(DateOnly? desde, DateOnly? hasta, int? categoriaId,
        CancellationToken ct = default)
    {
        ValidarPeriodo(desde, hasta);
        var hoy = _reloj.Hoy;

        var todas = await _reportes.ListarSolicitudesAsync(categoriaId, ct);
        var delPeriodo = FiltrarPeriodo(todas, desde, hasta).ToList();
        var recursos = await _recursos.ListarAsync(categoriaId, incluirDadosDeBaja: true, ct);

        // Préstamos efectivos: solicitudes que llegaron a entregarse
        var detallesPrestados = delPeriodo
            .Where(s => s.Estado is EstadoSolicitud.Entregada or EstadoSolicitud.Finalizada)
            .SelectMany(s => s.Detalles)
            .Where(d => categoriaId == null || d.Recurso.CategoriaId == categoriaId);

        return new ResumenResponse
        {
            Desde = desde,
            Hasta = hasta,
            CategoriaId = categoriaId,

            TotalSolicitudes = delPeriodo.Count,
            SolicitudesPorEstado = Enum.GetValues<EstadoSolicitud>()
                .ToDictionary(e => e.ToString(), e => delPeriodo.Count(s => s.Estado == e)),
            PrestamosActivos = delPeriodo.Count(s => s.Estado == EstadoSolicitud.Entregada && !s.EstaVencida(hoy)),
            PrestamosVencidos = delPeriodo.Count(s => s.EstaVencida(hoy)),

            TotalRecursos = recursos.Count,
            RecursosPorEstado = Enum.GetValues<EstadoRecurso>()
                .ToDictionary(e => e.ToString(), e => recursos.Count(r => r.EstadoOperativo == e)),
            // Situación actual, independiente del período
            RecursosPrestadosAhora = todas
                .Where(s => s.Estado == EstadoSolicitud.Entregada)
                .SelectMany(s => s.Detalles)
                .Where(d => d.FechaDevolucion == null && (categoriaId == null || d.Recurso.CategoriaId == categoriaId))
                .Select(d => d.RecursoId)
                .Distinct()
                .Count(),

            RecursosMasUtilizados = detallesPrestados
                .GroupBy(d => d.Recurso)
                .Select(g => new RecursoUsoResponse
                {
                    Codigo = g.Key.Codigo,
                    Nombre = g.Key.Nombre,
                    Categoria = g.Key.Categoria?.Nombre ?? string.Empty,
                    Prestamos = g.Count()
                })
                .OrderByDescending(r => r.Prestamos)
                .ThenBy(r => r.Codigo)
                .Take(5)
                .ToList()
        };
    }

    public async Task<List<PrestamoReporteResponse>> ListarPrestamosAsync(DateOnly? desde, DateOnly? hasta,
        int? categoriaId, string? estado, CancellationToken ct = default)
    {
        ValidarPeriodo(desde, hasta);

        estado = estado?.Trim();
        if (!string.IsNullOrWhiteSpace(estado) && !EstadosPrestamo.Contains(estado, StringComparer.OrdinalIgnoreCase))
            throw new DatosInvalidosException("estado", $"Estado no válido. Opciones: {string.Join(", ", EstadosPrestamo)}.");

        var hoy = _reloj.Hoy;
        var solicitudes = await _reportes.ListarSolicitudesAsync(categoriaId, ct);

        var filas = FiltrarPeriodo(solicitudes, desde, hasta)
            .Where(s => s.Estado is EstadoSolicitud.Entregada or EstadoSolicitud.Finalizada)
            .SelectMany(s => s.Detalles.Select(d => AFila(s, d, hoy)))
            .Where(f => categoriaId == null || f.CategoriaId == categoriaId)
            .Select(f => f.Fila)
            .Where(f => string.IsNullOrWhiteSpace(estado) || f.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f.FechaInicio)
            .ThenBy(f => f.SolicitudId)
            .ThenBy(f => f.RecursoCodigo)
            .ToList();

        return filas;
    }


    private static void ValidarPeriodo(DateOnly? desde, DateOnly? hasta)
    {
        if (desde.HasValue != hasta.HasValue)
            throw new DatosInvalidosException("desde", "Indica 'desde' y 'hasta' juntos, o ninguno.");
        if (desde > hasta)
            throw new DatosInvalidosException("hasta", "La fecha 'hasta' no puede ser anterior a 'desde'.");
    }

    private static IEnumerable<Solicitud> FiltrarPeriodo(IEnumerable<Solicitud> solicitudes, DateOnly? desde, DateOnly? hasta) =>
        desde is null || hasta is null
            ? solicitudes
            : solicitudes.Where(s => s.FechaInicio <= hasta && desde <= s.FechaFin);

    private static (PrestamoReporteResponse Fila, int CategoriaId) AFila(Solicitud s, DetalleSolicitud d, DateOnly hoy)
    {
        var devuelto = d.FechaDevolucion is not null;
        var vencido = !devuelto && s.FechaFin < hoy;

        var diasRetraso = devuelto
            ? Math.Max(0, DateOnly.FromDateTime(d.FechaDevolucion!.Value).DayNumber - s.FechaFin.DayNumber)
            : Math.Max(0, hoy.DayNumber - s.FechaFin.DayNumber);

        var fila = new PrestamoReporteResponse
        {
            SolicitudId = s.Id,
            RecursoCodigo = d.Recurso.Codigo,
            RecursoNombre = d.Recurso.Nombre,
            Categoria = d.Recurso.Categoria?.Nombre ?? string.Empty,
            Solicitante = s.Solicitante.Nombre,
            TipoSolicitante = s.Solicitante.TipoSolicitante?.ToString(),
            FechaInicio = s.FechaInicio,
            FechaFin = s.FechaFin,
            FechaEntrega = s.FechaEntrega,
            EntregadoPor = s.EntregadaPor?.Nombre,
            FechaDevolucion = d.FechaDevolucion,
            RecibidoPor = d.RecibidoPor?.Nombre,
            DevueltoConDano = d.DevueltoConDano,
            Estado = devuelto ? "Devuelto" : vencido ? "Vencido" : "Activo",
            DiasRetraso = diasRetraso
        };
        return (fila, d.Recurso.CategoriaId);
    }
}