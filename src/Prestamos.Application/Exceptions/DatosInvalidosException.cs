namespace Prestamos.Application.Exceptions;

public class DatosInvalidosException : Exception
{
    public IDictionary<string, string[]> Errores { get; }

    public DatosInvalidosException(IDictionary<string, string[]> errores)
        : base("Datos inválidos")
    {
        Errores = errores;
    }

    public DatosInvalidosException(string campo, string mensaje)
        : this(new Dictionary<string, string[]> { [campo] = [mensaje] }) { }
}