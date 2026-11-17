namespace Prestamos.Web.Services;

/// <summary>Error devuelto por la API (formato ProblemDetails), listo para mostrar.</summary>
public class Problema
{
    public string Titulo { get; set; } = "Error";
    public string? Detalle { get; set; }
    public List<string> Errores { get; set; } = [];

    /// <summary>Texto corto para notificaciones.</summary>
    public string Mensaje => !string.IsNullOrWhiteSpace(Detalle) && Detalle != Titulo
        ? Detalle
        : Errores.Count > 0 ? string.Join(" ", Errores) : Titulo;
}

/// <summary>Resultado de una llamada a la API: datos si salió bien, problema si falló.</summary>
public class Resultado<T>
{
    public bool Exito { get; private init; }
    public int Codigo { get; private init; }
    public T? Datos { get; private init; }
    public Problema? Problema { get; private init; }

    public static Resultado<T> Ok(T? datos, int codigo) => new() { Exito = true, Datos = datos, Codigo = codigo };
    public static Resultado<T> Fallo(int codigo, Problema problema) => new() { Exito = false, Codigo = codigo, Problema = problema };
}