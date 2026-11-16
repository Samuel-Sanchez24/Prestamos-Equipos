namespace Prestamos.Application.Exceptions;

public class AccesoDenegadoException : Exception
{
    public AccesoDenegadoException(string message = "No tienes permiso para realizar esta acción.")
        : base(message) { }
}