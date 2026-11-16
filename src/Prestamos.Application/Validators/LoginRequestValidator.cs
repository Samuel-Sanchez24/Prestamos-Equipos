using FluentValidation;
using Prestamos.Contracts.Auth;

namespace Prestamos.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Correo).NotEmpty().WithMessage("El correo es obligatorio.");
        RuleFor(x => x.Contrasena).NotEmpty().WithMessage("La contraseña es obligatoria.");
    }
}