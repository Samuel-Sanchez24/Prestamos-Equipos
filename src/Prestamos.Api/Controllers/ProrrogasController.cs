using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamos.Api.Extensions;
using Prestamos.Application.Services;
using Prestamos.Contracts.Prorrogas;

namespace Prestamos.Api.Controllers;

[ApiController]
[Authorize]
public class ProrrogasController : ControllerBase
{
    private readonly ProrrogaServicio _prorrogas;

    public ProrrogasController(ProrrogaServicio prorrogas)
    {
        _prorrogas = prorrogas;
    }

    [HttpPost("api/solicitudes/{id:int}/prorrogas")]
    [Authorize(Roles = "Solicitante")]
    [ProducesResponseType<ProrrogaResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Solicitar(int id, SolicitarProrrogaRequest req, CancellationToken ct)
    {
        var prorroga = await _prorrogas.SolicitarAsync(id, req, User.ObtenerUsuarioId(), ct);
        return CreatedAtAction(nameof(Obtener), new { id = prorroga.Id }, prorroga);
    }

    [HttpGet("api/prorrogas")]
    [ProducesResponseType<List<ProrrogaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string? estado, CancellationToken ct) =>
        Ok(await _prorrogas.ListarAsync(estado, User.ObtenerUsuarioId(), User.ObtenerRol(), ct));

    [HttpGet("api/prorrogas/{id:int}")]
    [ProducesResponseType<ProrrogaResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Obtener(int id, CancellationToken ct) =>
        Ok(await _prorrogas.ObtenerAsync(id, User.ObtenerUsuarioId(), User.ObtenerRol(), ct));

    [HttpPost("api/prorrogas/{id:int}/aprobar")]
    [Authorize(Roles = "Tecnico")]
    [ProducesResponseType<ProrrogaResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Aprobar(int id, CancellationToken ct) =>
        Ok(await _prorrogas.AprobarAsync(id, User.ObtenerUsuarioId(), ct));

    [HttpPost("api/prorrogas/{id:int}/rechazar")]
    [Authorize(Roles = "Tecnico")]
    [ProducesResponseType<ProrrogaResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Rechazar(int id, RechazarProrrogaRequest req, CancellationToken ct) =>
        Ok(await _prorrogas.RechazarAsync(id, req, User.ObtenerUsuarioId(), ct));
}