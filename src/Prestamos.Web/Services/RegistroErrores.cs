using Microsoft.Extensions.Logging;

namespace Prestamos.Web.Services;

/// <summary>Un error ocurrido en la aplicación (respuesta de la API o fallo de la interfaz).</summary>
public record EntradaError(DateTime Fecha, string Origen, int? Codigo, string Titulo, string? Detalle, string? Ruta);

/// <summary>
/// Guarda en memoria los últimos errores para mostrarlos en la página "Registro de errores"
/// y los escribe en la consola del navegador (F12).
/// </summary>
public class RegistroErrores
{
    private const int Maximo = 100;
    private readonly List<EntradaError> _errores = [];
    private readonly ILogger<RegistroErrores> _logger;

    public RegistroErrores(ILogger<RegistroErrores> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<EntradaError> Errores => _errores;
    public EntradaError? Ultimo => _errores.Count > 0 ? _errores[0] : null;

    public event Action? Cambio;

    public void Registrar(string origen, int? codigo, string titulo, string? detalle = null, string? ruta = null)
    {
        _errores.Insert(0, new EntradaError(DateTime.Now, origen, codigo, titulo, detalle, ruta));
        if (_errores.Count > Maximo) _errores.RemoveAt(_errores.Count - 1);

        _logger.LogWarning("[{Origen}] {Codigo} {Titulo} · {Detalle} · {Ruta}", origen, codigo, titulo, detalle, ruta);
        Cambio?.Invoke();
    }

    public void Limpiar()
    {
        _errores.Clear();
        Cambio?.Invoke();
    }
}