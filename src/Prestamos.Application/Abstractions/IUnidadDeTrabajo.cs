namespace Prestamos.Application.Abstractions;

public interface IUnidadDeTrabajo
{
    Task GuardarCambiosAsync(CancellationToken ct = default);
    Task<T> EnTransaccionAsync<T>(Func<Task<T>> operacion, CancellationToken ct = default);
}