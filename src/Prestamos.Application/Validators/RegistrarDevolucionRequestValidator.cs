using FluentValidation;
using Prestamos.Contracts.Solicitudes;

namespace Prestamos.Application.Validators;

public class RegistrarDevolucionRequestValidator : AbstractValidator<RegistrarDevolucionRequest>
{
    public RegistrarDevolucionRequestValidator()
    {
        RuleFor(x => x.RecursoId).GreaterThan(0).WithMessage("El recurso es obligatorio.");
        RuleFor(x => x.Observacion).MaximumLength(300).WithMessage("La observación no puede superar 300 caracteres.");
        RuleFor(x => x.Observacion)
            .NotEmpty().WithMessage("Describe el daño en la observación.")
            .When(x => x.DevueltoConDano);
    }
}