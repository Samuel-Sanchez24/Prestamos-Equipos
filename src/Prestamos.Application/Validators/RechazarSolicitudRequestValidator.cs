using FluentValidation;
using Prestamos.Contracts.Solicitudes;

namespace Prestamos.Application.Validators;

public class RechazarSolicitudRequestValidator : AbstractValidator<RechazarSolicitudRequest>
{
    public RechazarSolicitudRequestValidator()
    {
        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("El motivo del rechazo es obligatorio.")
            .Length(10, 300).WithMessage("El motivo debe tener entre 10 y 300 caracteres.");
    }
}