using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Application.Validators;
using Prestamos.Contracts.Recursos;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Application.Services;

public class CategoriaServicio
{
    private readonly ICategoriaRepositorio _categorias;
    private readonly IUnidadDeTrabajo _uow;
    private readonly IValidator<CrearCategoriaRequest> _validador;

    public CategoriaServicio(ICategoriaRepositorio categorias, IUnidadDeTrabajo uow, IValidator<CrearCategoriaRequest> validador)
    {
        _categorias = categorias;
        _uow = uow;
        _validador = validador;
    }

    public async Task<List<CategoriaResponse>> ListarAsync(CancellationToken ct = default) =>
        (await _categorias.ListarAsync(ct)).Select(c => c.ARespuesta()).ToList();

    public async Task<CategoriaResponse> CrearAsync(CrearCategoriaRequest req, CancellationToken ct = default)
    {
        await _validador.ValidarOFallarAsync(req, ct);

        var nombre = req.Nombre.Trim();
        if (await _categorias.ExisteNombreAsync(nombre, ct))
            throw new BusinessRuleException($"Ya existe la categoría '{nombre}'.");

        var categoria = new Categoria { Nombre = nombre };
        _categorias.Agregar(categoria);
        await _uow.GuardarCambiosAsync(ct);

        return categoria.ARespuesta();
    }
}