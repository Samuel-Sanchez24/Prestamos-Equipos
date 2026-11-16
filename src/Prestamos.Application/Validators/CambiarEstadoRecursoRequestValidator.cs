using FluentValidation;
using Prestamos.Contracts.Recursos;
using Prestamos.Domain.Enums;

namespace Prestamos.Application.Validators;

public class CambiarEstadoRecursoRequestValidator : AbstractValidator<CambiarEstadoRecursoRequest>
{
    public CambiarEstadoRecursoRequestValidator()
    {
        RuleFor(x => x.Estado).NotEmpty().WithMessage("El estado es obligatorio.")
                              .ValorDe<CambiarEstadoRecursoRequest, EstadoRecurso>();
        RuleFor(x => x.Observacion).MaximumLength(300).WithMessage("La observación no puede superar 300 caracteres.");
    }
}