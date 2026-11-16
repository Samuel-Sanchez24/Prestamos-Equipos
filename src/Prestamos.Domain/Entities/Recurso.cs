using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Domain.Entities;

public class Recurso
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string? Descripcion { get; set; }
    public string? Ubicacion { get; set; }
    public EstadoRecurso EstadoOperativo { get; set; } = EstadoRecurso.Disponible;
    public DateTime FechaRegistro { get; set; }

    public Categoria Categoria { get; set; } = null!;
    public ICollection<DetalleSolicitud> Detalles { get; set; } = new List<DetalleSolicitud>();

    // ---- Comportamiento (RN-01, RN-15) ----

    public void MarcarDanado()
    {
        if (EstadoOperativo == EstadoRecurso.DadoDeBaja)
            throw new BusinessRuleException($"El recurso {Codigo} está dado de baja.");
        if (EstadoOperativo == EstadoRecurso.Danado)
            throw new BusinessRuleException($"El recurso {Codigo} ya está reportado como dañado.");
        EstadoOperativo = EstadoRecurso.Danado;
    }

    public void MarcarReparado()
    {
        if (EstadoOperativo != EstadoRecurso.Danado)
            throw new BusinessRuleException($"Solo un recurso dañado puede marcarse como reparado. Estado actual de {Codigo}: {EstadoOperativo}.");
        EstadoOperativo = EstadoRecurso.Disponible;
    }

    public void DarDeBaja()
    {
        if (EstadoOperativo == EstadoRecurso.DadoDeBaja)
            throw new BusinessRuleException($"El recurso {Codigo} ya está dado de baja.");
        EstadoOperativo = EstadoRecurso.DadoDeBaja;
    }
}