using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamos.Api.Extensions;
using Prestamos.Application.Services;
using Prestamos.Contracts.Auth;
using Prestamos.Contracts.Usuarios;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthServicio _auth;

    public AuthController(AuthServicio auth)
    {
        _auth = auth;
    }

    [HttpPost("registro")]
    [AllowAnonymous]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar(RegistroRequest req, CancellationToken ct)
    {
        var usuario = await _auth.RegistrarAsync(req, ct);
        return Created($"/api/usuarios/{usuario.Id}", usuario);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<SesionResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequest req, CancellationToken ct) =>
        Ok(await _auth.IniciarSesionAsync(req, ct));

    [HttpGet("yo")]
    [Authorize]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Yo(CancellationToken ct) =>
        Ok(await _auth.ObtenerActualAsync(User.ObtenerUsuarioId(), ct));
}