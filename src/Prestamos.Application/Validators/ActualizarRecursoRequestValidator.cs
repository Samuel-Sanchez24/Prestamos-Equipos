using FluentValidation;
using Prestamos.Contracts.Recursos;

namespace Prestamos.Application.Validators;

public class ActualizarRecursoRequestValidator : AbstractValidator<ActualizarRecursoRequest>
{
    public ActualizarRecursoRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");
        RuleFor(x => x.CategoriaId).GreaterThan(0).WithMessage("La categoría es obligatoria.");
        RuleFor(x => x.Descripcion).MaximumLength(300).WithMessage("La descripción no puede superar 300 caracteres.");
        RuleFor(x => x.Ubicacion).MaximumLength(100).WithMessage("La ubicación no puede superar 100 caracteres.");
    }
}