using System.Globalization;

namespace Prestamos.Web.Services;

/// <summary>Conversión de fechas entre los campos de la interfaz (yyyy-MM-dd) y la API.</summary>
public static class Formato
{
    public static string Iso(DateOnly fecha) => fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static DateOnly? LeerFecha(string? texto) =>
        DateOnly.TryParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var f) ? f : null;

    public static string Corta(DateOnly fecha) => fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    public static string Corta(DateTime fecha) => fecha.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
    public static string Corta(DateTime? fecha) => fecha is null ? "—" : Corta(fecha.Value);

    public static DateOnly Hoy => DateOnly.FromDateTime(DateTime.UtcNow.AddHours(-5)); // Hora de Colombia
}