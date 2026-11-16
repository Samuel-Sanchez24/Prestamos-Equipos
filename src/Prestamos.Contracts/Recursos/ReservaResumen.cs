namespace Prestamos.Contracts.Recursos;

public class ReservaResumen
{
    public int SolicitudId { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
}