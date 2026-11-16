using Microsoft.AspNetCore.Identity;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;

namespace Prestamos.Api.Security;

public class HashContrasena : IHashContrasena
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    public string Generar(string contrasena) => _hasher.HashPassword(null!, contrasena);

    public bool Verificar(string hash, string contrasena) =>
        _hasher.VerifyHashedPassword(null!, hash, contrasena) != PasswordVerificationResult.Failed;
}