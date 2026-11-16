namespace Prestamos.Contracts.Solicitudes;

public class CrearSolicitudRequest
{
    public List<int> RecursoIds { get; set; } = [];
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
}