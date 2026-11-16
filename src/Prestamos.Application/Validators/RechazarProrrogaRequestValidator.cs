using FluentValidation;
using Prestamos.Contracts.Prorrogas;

namespace Prestamos.Application.Validators;

public class RechazarProrrogaRequestValidator : AbstractValidator<RechazarProrrogaRequest>
{
    public RechazarProrrogaRequestValidator()
    {
        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("El motivo del rechazo es obligatorio.")
            .Length(10, 300).WithMessage("El motivo debe tener entre 10 y 300 caracteres.");
    }
}