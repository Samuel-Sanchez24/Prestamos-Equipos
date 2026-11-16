using FluentValidation;
using Prestamos.Contracts.Auth;

namespace Prestamos.Application.Validators;

public class RegistroRequestValidator : AbstractValidator<RegistroRequest>
{
    public RegistroRequestValidator()
    {
        RuleFor(x => x.Nombre).Nombre();
        RuleFor(x => x.Documento).Documento();
        RuleFor(x => x.Correo).Correo();
        RuleFor(x => x.Contrasena).Contrasena();
        RuleFor(x => x.TipoSolicitante)
            .Must(t => t is "Estudiante" or "Docente")
            .WithMessage("El tipo de solicitante debe ser Estudiante o Docente.");
    }
}