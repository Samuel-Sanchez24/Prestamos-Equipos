using Microsoft.EntityFrameworkCore;
using Prestamos.Application.Abstractions;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Persistence.Context;

namespace Prestamos.Persistence.Repositories;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly PrestamosDbContext _db;

    public UsuarioRepositorio(PrestamosDbContext db)
    {
        _db = db;
    }

    public Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default) =>
        _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo, ct);

    public Task<bool> ExisteCorreoAsync(string correo, CancellationToken ct = default) =>
        _db.Usuarios.AnyAsync(u => u.Correo == correo, ct);

    public Task<bool> ExisteDocumentoAsync(string documento, CancellationToken ct = default) =>
        _db.Usuarios.AnyAsync(u => u.Documento == documento, ct);

    public Task<List<Usuario>> ListarAsync(Rol? rol, CancellationToken ct = default) =>
        _db.Usuarios
           .Where(u => rol == null || u.Rol == rol)
           .OrderBy(u => u.Nombre)
           .ToListAsync(ct);

    public void Agregar(Usuario usuario) => _db.Usuarios.Add(usuario);
}