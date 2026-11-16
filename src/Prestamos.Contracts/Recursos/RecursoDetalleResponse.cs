namespace Prestamos.Contracts.Recursos;

public class RecursoDetalleResponse : RecursoResponse
{
    public List<ReservaResumen> ProximasReservas { get; set; } = [];
}