namespace Prestamos.Contracts.Prorrogas;

public class SolicitarProrrogaRequest
{
    public DateOnly NuevaFechaFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
}