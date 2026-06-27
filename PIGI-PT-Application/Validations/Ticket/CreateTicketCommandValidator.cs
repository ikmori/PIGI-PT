using FluentValidation;
using PIGI_PT_Application.Commands.Ticket;

namespace PIGI_PT_Application.Validations.Ticket
{
    public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketCommandValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .MaximumLength(200).WithMessage("El título no puede superar los 200 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MinimumLength(10).WithMessage("La descripción debe tener al menos 10 caracteres.")
                .MaximumLength(5000).WithMessage("La descripción no puede superar los 5000 caracteres.");

            RuleFor(x => x.InquilinoId)
                .NotEmpty().WithMessage("El identificador del inquilino es obligatorio.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El identificador del usuario creador es obligatorio.");
        }
    }
}
