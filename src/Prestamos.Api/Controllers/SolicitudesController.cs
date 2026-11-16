using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamos.Api.Extensions;
using Prestamos.Application.Services;
using Prestamos.Contracts.Reportes;
using Prestamos.Contracts.Solicitudes;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/solicitudes")]
[Authorize]
public class SolicitudesController : ControllerBase
{
    private readonly SolicitudServicio _solicitudes;

    public SolicitudesController(SolicitudServicio solicitudes)
    {
        _solicitudes = solicitudes;
    }

    [HttpPost]
    [Authorize(Roles = "Solicitante")]
    [ProducesResponseType<SolicitudResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear(CrearSolicitudRequest req, CancellationToken ct)
    {
        var solicitud = await _solicitudes.CrearAsync(req, User.ObtenerUsuarioId(), ct);
        return CreatedAtAction(nameof(Obtener), new { id = solicitud.Id }, solicitud);
    }

    [HttpGet]
    [ProducesResponseType<List<SolicitudResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string? estado, [FromQuery] bool vencidas = false,
        [FromQuery] int? solicitanteId = null, [FromQuery] DateOnly? desde = null, [FromQuery] DateOnly? hasta = null,
        CancellationToken ct = default) =>
        Ok(await _solicitudes.ListarAsync(estado, vencidas, solicitanteId, desde, hasta,
            User.ObtenerUsuarioId(), User.ObtenerRol(), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType<SolicitudResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Obtener(int id, CancellationToken ct) =>
        Ok(await _solicitudes.ObtenerAsync(id, User.ObtenerUsuarioId(), User.ObtenerRol(), ct));

    [HttpPost("{id:int}/cancelar")]
    [Authorize(Roles = "Solicitante")]
    [ProducesResponseType<SolicitudResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancelar(int id, CancellationToken ct) =>
        Ok(await _solicitudes.CancelarAsync(id, User.ObtenerUsuarioId(), ct));

    [HttpGet("{id:int}/movimientos")]
    [ProducesResponseType<List<MovimientoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Movimientos(int id, CancellationToken ct) =>
        Ok(await _solicitudes.ListarMovimientosAsync(id, User.ObtenerUsuarioId(), User.ObtenerRol(), ct));
}