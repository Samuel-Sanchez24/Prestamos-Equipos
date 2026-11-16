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
    private readonly GestionSolicitudServicio _gestion;

    public SolicitudesController(SolicitudServicio solicitudes, GestionSolicitudServicio gestion)
    {
        _solicitudes = solicitudes;
        _gestion = gestion;
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


    [HttpPost("{id:int}/aprobar")]
    [Authorize(Roles = "Tecnico")]
    [ProducesResponseType<SolicitudResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Aprobar(int id, CancellationToken ct) =>
        Ok(await _gestion.AprobarAsync(id, User.ObtenerUsuarioId(), ct));

    [HttpPost("{id:int}/rechazar")]
    [Authorize(Roles = "Tecnico")]
    [ProducesResponseType<SolicitudResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Rechazar(int id, RechazarSolicitudRequest req, CancellationToken ct) =>
        Ok(await _gestion.RechazarAsync(id, req, User.ObtenerUsuarioId(), ct));

    [HttpPost("{id:int}/entregar")]
    [Authorize(Roles = "Tecnico")]
    [ProducesResponseType<SolicitudResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Entregar(int id, CancellationToken ct) =>
        Ok(await _gestion.EntregarAsync(id, User.ObtenerUsuarioId(), ct));

    [HttpPost("{id:int}/no-recogida")]
    [Authorize(Roles = "Tecnico")]
    [ProducesResponseType<SolicitudResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> NoRecogida(int id, CancellationToken ct) =>
        Ok(await _gestion.MarcarNoRecogidaAsync(id, User.ObtenerUsuarioId(), ct));

    [HttpPost("{id:int}/devoluciones")]
    [Authorize(Roles = "Tecnico")]
    [ProducesResponseType<SolicitudResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Devolver(int id, RegistrarDevolucionRequest req, CancellationToken ct) =>
        Ok(await _gestion.RegistrarDevolucionAsync(id, req, User.ObtenerUsuarioId(), ct));


}