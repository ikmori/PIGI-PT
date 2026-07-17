using FluentValidation;
using PIGI_PT_Application.Commands.Usuario;

namespace PIGI_PT_Application.Validations.Usuario
{
    public class CreateUsuarioCommandValidator : AbstractValidator<CreateUsuarioCommand>
    {
        public CreateUsuarioCommandValidator()
        {
            RuleFor(x => x.InquilinoId)
                .NotEmpty().WithMessage("El identificador del inquilino es obligatorio.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("El nombre completo es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre completo no puede superar los 150 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El formato del correo electrónico es inválido.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MinimumLength(4).WithMessage("El nombre de usuario debe tener al menos 4 caracteres.")
                .MaximumLength(50).WithMessage("El nombre de usuario no puede superar los 50 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

            RuleFor(x => x.RolValor)
                .Must(r => r == 1 || r == 3 || r == 4).WithMessage("El rol asignado debe ser válido (1=Admin, 3=DepartamentoTecnologia, 4=Usuario).");
        }
    }
}
