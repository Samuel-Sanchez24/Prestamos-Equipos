namespace Prestamos.Application.Abstractions;

public interface IHashContrasena
{
    string Generar(string contrasena);
    bool Verificar(string hash, string contrasena);
}