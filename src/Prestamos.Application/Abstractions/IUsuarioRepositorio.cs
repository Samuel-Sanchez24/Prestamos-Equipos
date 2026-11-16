using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;

namespace Prestamos.Application.Abstractions;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken ct = default);
    Task<bool> ExisteCorreoAsync(string correo, CancellationToken ct = default);
    Task<bool> ExisteDocumentoAsync(string documento, CancellationToken ct = default);
    Task<List<Usuario>> ListarAsync(Rol? rol, CancellationToken ct = default);
    void Agregar(Usuario usuario);
}