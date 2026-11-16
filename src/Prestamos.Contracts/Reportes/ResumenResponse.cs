namespace Prestamos.Contracts.Reportes;

public class ResumenResponse
{
    public DateOnly? Desde { get; set; }
    public DateOnly? Hasta { get; set; }
    public int? CategoriaId { get; set; }

    public int TotalSolicitudes { get; set; }
    public Dictionary<string, int> SolicitudesPorEstado { get; set; } = [];
    public int PrestamosActivos { get; set; }
    public int PrestamosVencidos { get; set; }

    public int TotalRecursos { get; set; }
    public Dictionary<string, int> RecursosPorEstado { get; set; } = [];
    public int RecursosPrestadosAhora { get; set; }

    // Uso
    public List<RecursoUsoResponse> RecursosMasUtilizados { get; set; } = [];
}