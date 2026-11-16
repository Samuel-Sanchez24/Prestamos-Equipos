using Prestamos.Application.Abstractions;

namespace Prestamos.Api.Services;

public class RelojColombia : IReloj
{
    public DateTime Ahora => DateTime.UtcNow.AddHours(-5);
    public DateOnly Hoy => DateOnly.FromDateTime(Ahora);
}