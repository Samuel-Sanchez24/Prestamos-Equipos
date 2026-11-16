namespace Prestamos.Application.Abstractions;

public interface IUnidadDeTrabajo
{
    Task GuardarCambiosAsync(CancellationToken ct = default);
}