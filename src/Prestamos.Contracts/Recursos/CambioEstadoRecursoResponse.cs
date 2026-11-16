namespace Prestamos.Contracts.Recursos;

public class CambioEstadoRecursoResponse
{
    public RecursoResponse Recurso { get; set; } = new();
    public List<ReservaResumen> ReservasAfectadas { get; set; } = [];
}