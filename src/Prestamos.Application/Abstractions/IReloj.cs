namespace Prestamos.Application.Abstractions;

public interface IReloj
{
    DateTime Ahora { get; }
    DateOnly Hoy { get; }
}