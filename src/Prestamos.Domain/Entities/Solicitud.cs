using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Domain.Entities;

public class Solicitud
{
    public int Id { get; set; }
    public int SolicitanteId { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;
    public DateTime FechaRegistro { get; set; }


    public int? EvaluadaPorId { get; set; }
    public DateTime? FechaEvaluacion { get; set; }
    public string? MotivoRechazo { get; set; }


    public int? EntregadaPorId { get; set; }
    public DateTime? FechaEntrega { get; set; }

    public Usuario Solicitante { get; set; } = null!;
    public Usuario? EvaluadaPor { get; set; }
    public Usuario? EntregadaPor { get; set; }
    public ICollection<DetalleSolicitud> Detalles { get; set; } = new List<DetalleSolicitud>();
    public ICollection<Prorroga> Prorrogas { get; set; } = new List<Prorroga>();



    public bool EstaVencida(DateOnly hoy) =>
        Estado == EstadoSolicitud.Entregada && FechaFin < hoy && Detalles.Any(d => d.FechaDevolucion == null);

    public void Cancelar()
    {
        if (Estado is not (EstadoSolicitud.Pendiente or EstadoSolicitud.Aprobada))
            throw new BusinessRuleException(
                $"La solicitud {Id} no se puede cancelar porque está en estado {Estado}. Solo se cancelan solicitudes pendientes o aprobadas.");
        Estado = EstadoSolicitud.Cancelada;
    }

    public void Aprobar(int tecnicoId, DateTime ahora)
    {
        ExigirEstado(EstadoSolicitud.Pendiente, "aprobar");
        Estado = EstadoSolicitud.Aprobada;
        EvaluadaPorId = tecnicoId;
        FechaEvaluacion = ahora;
    }

    public void Rechazar(int tecnicoId, DateTime ahora, string motivo)
    {
        ExigirEstado(EstadoSolicitud.Pendiente, "rechazar");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new BusinessRuleException("El motivo del rechazo es obligatorio.");
        Estado = EstadoSolicitud.Rechazada;
        EvaluadaPorId = tecnicoId;
        FechaEvaluacion = ahora;
        MotivoRechazo = motivo.Trim();
    }

    public void Entregar(int tecnicoId, DateTime ahora, DateOnly hoy)
    {
        ExigirEstado(EstadoSolicitud.Aprobada, "entregar");
        if (hoy < FechaInicio)
            throw new BusinessRuleException(
                $"La solicitud {Id} no puede entregarse antes de su fecha de inicio ({FechaInicio:yyyy-MM-dd}).");
        if (hoy > FechaFin)
            throw new BusinessRuleException(
                $"El período de la solicitud {Id} terminó el {FechaFin:yyyy-MM-dd}. Márcala como no recogida.");
        Estado = EstadoSolicitud.Entregada;
        EntregadaPorId = tecnicoId;
        FechaEntrega = ahora;
    }

    public void MarcarNoRecogida()
    {
        ExigirEstado(EstadoSolicitud.Aprobada, "marcar como no recogida");
        Estado = EstadoSolicitud.NoRecogida;
    }


    public DetalleSolicitud RegistrarDevolucion(int recursoId, int tecnicoId, DateTime ahora, bool conDano, string? observacion)
    {
        ExigirEstado(EstadoSolicitud.Entregada, "registrar devoluciones en");

        var detalle = Detalles.FirstOrDefault(d => d.RecursoId == recursoId)
            ?? throw new BusinessRuleException($"El recurso {recursoId} no pertenece a la solicitud {Id}.");
        if (detalle.FechaDevolucion is not null)
            throw new BusinessRuleException($"El recurso {detalle.Recurso?.Codigo ?? recursoId.ToString()} ya fue devuelto.");

        detalle.FechaDevolucion = ahora;
        detalle.RecibidoPorId = tecnicoId;
        detalle.DevueltoConDano = conDano;
        detalle.ObservacionDevolucion = observacion?.Trim();

        if (Detalles.All(d => d.FechaDevolucion is not null))
            Estado = EstadoSolicitud.Finalizada;

        return detalle;
    }

    private void ExigirEstado(EstadoSolicitud requerido, string accion)
    {
        if (Estado != requerido)
            throw new BusinessRuleException(
                $"No se puede {accion} la solicitud {Id} porque está en estado {Estado}. Debe estar en {requerido}.");
    }

    public Prorroga SolicitarProrroga(DateOnly nuevaFechaFin, string motivo, DateTime ahora)
    {
        ExigirEstado(EstadoSolicitud.Entregada, "solicitar prórroga de");
        if (Prorrogas.Any(p => p.Estado == EstadoProrroga.Pendiente))
            throw new BusinessRuleException($"La solicitud {Id} ya tiene una prórroga pendiente de evaluación.");
        if (nuevaFechaFin <= FechaFin)
            throw new BusinessRuleException(
                $"La nueva fecha de fin debe ser posterior a la actual ({FechaFin:yyyy-MM-dd}).");

        var prorroga = new Prorroga
        {
            FechaFinAnterior = FechaFin,
            FechaFinSolicitada = nuevaFechaFin,
            Motivo = motivo.Trim(),
            Estado = EstadoProrroga.Pendiente,
            FechaSolicitud = ahora
        };
        Prorrogas.Add(prorroga);
        return prorroga;
    }

    public void AmpliarFechaFin(DateOnly nuevaFechaFin)
    {
        ExigirEstado(EstadoSolicitud.Entregada, "ampliar la fecha de fin de");
        if (nuevaFechaFin <= FechaFin)
            throw new BusinessRuleException(
                $"La nueva fecha de fin debe ser posterior a la actual ({FechaFin:yyyy-MM-dd}).");
        FechaFin = nuevaFechaFin;
    }
}