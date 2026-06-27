using FluentValidation;
using PIGI_PT_Application.Commands.Inquilino;

namespace PIGI_PT_Application.Validations.Inquilino
{
    public class CreateInquilinoCommandValidator : AbstractValidator<CreateInquilinoCommand>
    {
        public CreateInquilinoCommandValidator()
        {
            RuleFor(x => x.NombreComercial)
                .NotEmpty().WithMessage("El nombre comercial es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre comercial no puede superar los 150 caracteres.");

            RuleFor(x => x.DominioRed)
                .NotEmpty().WithMessage("El dominio de red es obligatorio.")
                .MaximumLength(100).WithMessage("El dominio de red no puede superar los 100 caracteres.")
                .Matches(@"^[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("El dominio de red debe tener un formato de dominio válido (ej: empresa.com).");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El identificador del usuario creador es obligatorio.");
        }
    }
}
