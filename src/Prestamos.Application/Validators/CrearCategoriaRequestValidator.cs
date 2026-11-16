using FluentValidation;
using Prestamos.Contracts.Recursos;

namespace Prestamos.Application.Validators;

public class CrearCategoriaRequestValidator : AbstractValidator<CrearCategoriaRequest>
{
    public CrearCategoriaRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(60).WithMessage("El nombre no puede superar 60 caracteres.");
    }
}