using FluentValidation;
using Prestamos.Contracts.Usuarios;

namespace Prestamos.Application.Validators;

public class CrearPersonalRequestValidator : AbstractValidator<CrearPersonalRequest>
{
    public CrearPersonalRequestValidator()
    {
        RuleFor(x => x.Nombre).Nombre();
        RuleFor(x => x.Documento).Documento();
        RuleFor(x => x.Correo).Correo();
        RuleFor(x => x.Contrasena).Contrasena();
        RuleFor(x => x.Rol)
            .Must(r => r is "Tecnico" or "Coordinacion")
            .WithMessage("El rol debe ser Tecnico o Coordinacion.");
    }
}