using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Contracts.Prorrogas;

namespace Prestamos.Application.Validators;

public class SolicitarProrrogaRequestValidator : AbstractValidator<SolicitarProrrogaRequest>
{
    public SolicitarProrrogaRequestValidator(IReloj reloj)
    {
        RuleFor(x => x.NuevaFechaFin)
            .NotEqual(default(DateOnly)).WithMessage("La nueva fecha de fin es obligatoria.")
            .GreaterThanOrEqualTo(_ => reloj.Hoy).WithMessage("La nueva fecha de fin no puede ser anterior a hoy.");
        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("El motivo es obligatorio.")
            .Length(10, 300).WithMessage("El motivo debe tener entre 10 y 300 caracteres.");
    }
}