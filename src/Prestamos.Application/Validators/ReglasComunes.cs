using FluentValidation;

namespace Prestamos.Application.Validators;

public static class ReglasComunes
{
    public static IRuleBuilderOptions<T, string> Nombre<T>(this IRuleBuilder<T, string> r) =>
        r.NotEmpty().WithMessage("El nombre es obligatorio.")
         .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

    public static IRuleBuilderOptions<T, string> Documento<T>(this IRuleBuilder<T, string> r) =>
        r.NotEmpty().WithMessage("El documento es obligatorio.")
         .Matches(@"^\d{6,20}$").WithMessage("El documento debe tener entre 6 y 20 dígitos.");

    public static IRuleBuilderOptions<T, string> Correo<T>(this IRuleBuilder<T, string> r) =>
        r.NotEmpty().WithMessage("El correo es obligatorio.")
         .EmailAddress().WithMessage("El correo no tiene un formato válido.")
         .MaximumLength(150).WithMessage("El correo no puede superar 150 caracteres.");

    public static IRuleBuilderOptions<T, string> Contrasena<T>(this IRuleBuilder<T, string> r) =>
        r.NotEmpty().WithMessage("La contraseña es obligatoria.")
         .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
         .Matches("[A-Za-z]").WithMessage("La contraseña debe incluir al menos una letra.")
         .Matches("[0-9]").WithMessage("La contraseña debe incluir al menos un número.");
}