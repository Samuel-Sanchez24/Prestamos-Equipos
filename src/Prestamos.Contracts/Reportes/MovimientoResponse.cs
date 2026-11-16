namespace Prestamos.Contracts.Reportes;

public class MovimientoResponse
{
    public DateTime Fecha { get; set; }
    public string? EstadoAnterior { get; set; }
    public string EstadoNuevo { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string? Observacion { get; set; }
}