namespace Prestamos.Application.Exceptions;

public class NoAutenticadoException : Exception
{
    public NoAutenticadoException(string message = "Correo o contraseña incorrectos.") : base(message) { }
}