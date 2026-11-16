using FluentValidation;
using Prestamos.Application.Exceptions;

namespace Prestamos.Application.Validators;

public static class ValidacionExtensions
{
    public static async Task ValidarOFallarAsync<T>(this IValidator<T> validador, T instancia, CancellationToken ct = default)
    {
        var resultado = await validador.ValidateAsync(instancia, ct);
        if (resultado.IsValid) return;

        var errores = resultado.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        throw new DatosInvalidosException(errores);
    }
}