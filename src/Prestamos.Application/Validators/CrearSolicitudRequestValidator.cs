using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Contracts.Solicitudes;

namespace Prestamos.Application.Validators;

public class CrearSolicitudRequestValidator : AbstractValidator<CrearSolicitudRequest>
{
    public CrearSolicitudRequestValidator(IReloj reloj)
    {
        RuleFor(x => x.RecursoIds)
            .NotEmpty().WithMessage("Debe incluir al menos un recurso.")
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("No repitas recursos en la misma solicitud.")
            .Must(ids => ids.All(id => id > 0)).WithMessage("Los ids de recurso deben ser mayores que cero.");

        RuleFor(x => x.FechaInicio)
            .NotEqual(default(DateOnly)).WithMessage("La fecha de inicio es obligatoria.")
            .GreaterThanOrEqualTo(_ => reloj.Hoy).WithMessage("La fecha de inicio no puede ser anterior a hoy.");

        RuleFor(x => x.FechaFin)
            .NotEqual(default(DateOnly)).WithMessage("La fecha de fin es obligatoria.")
            .GreaterThan(x => x.FechaInicio).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("El motivo es obligatorio.")
            .Length(10, 300).WithMessage("El motivo debe tener entre 10 y 300 caracteres.");
    }
}