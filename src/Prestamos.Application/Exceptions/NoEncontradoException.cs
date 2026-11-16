namespace Prestamos.Application.Exceptions;

public class NoEncontradoException : Exception
{
    public NoEncontradoException(string message) : base(message) { }
}