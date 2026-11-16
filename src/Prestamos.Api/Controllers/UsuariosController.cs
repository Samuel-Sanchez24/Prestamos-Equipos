using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamos.Application.Services;
using Prestamos.Contracts.Usuarios;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioServicio _usuarios;

    public UsuariosController(UsuarioServicio usuarios)
    {
        _usuarios = usuarios;
    }

    [HttpPost]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearPersonal(CrearPersonalRequest req, CancellationToken ct)
    {
        var usuario = await _usuarios.CrearPersonalAsync(req, ct);
        return CreatedAtAction(nameof(Obtener), new { id = usuario.Id }, usuario);
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? rol, CancellationToken ct) =>
        Ok(await _usuarios.ListarAsync(rol, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id, CancellationToken ct) =>
        Ok(await _usuarios.ObtenerAsync(id, ct));

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, ActualizarUsuarioRequest req, CancellationToken ct) =>
        Ok(await _usuarios.ActualizarAsync(id, req, ct));
}