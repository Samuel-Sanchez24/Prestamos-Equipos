namespace Prestamos.Contracts.Solicitudes;

public class RegistrarDevolucionRequest
{
    public int RecursoId { get; set; }
    public bool DevueltoConDano { get; set; }
    public string? Observacion { get; set; }
}