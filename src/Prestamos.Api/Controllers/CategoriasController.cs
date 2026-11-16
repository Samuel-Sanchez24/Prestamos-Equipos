using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamos.Application.Services;
using Prestamos.Contracts.Recursos;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/categorias")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly CategoriaServicio _categorias;

    public CategoriasController(CategoriaServicio categorias)
    {
        _categorias = categorias;
    }

    [HttpGet]
    [ProducesResponseType<List<CategoriaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct) => Ok(await _categorias.ListarAsync(ct));

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType<CategoriaResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear(CrearCategoriaRequest req, CancellationToken ct)
    {
        var categoria = await _categorias.CrearAsync(req, ct);
        return Created($"/api/categorias/{categoria.Id}", categoria);
    }
}