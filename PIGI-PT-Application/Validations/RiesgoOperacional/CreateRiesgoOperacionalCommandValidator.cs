using FluentValidation;
using PIGI_PT_Application.Commands.RiesgoOperacional;

namespace PIGI_PT_Application.Validations.RiesgoOperacional
{
    public class CreateRiesgoOperacionalCommandValidator : AbstractValidator<CreateRiesgoOperacionalCommand>
    {
        public CreateRiesgoOperacionalCommandValidator()
        {
            RuleFor(x => x.InquilinoId)
                .NotEmpty().WithMessage("El identificador del inquilino es obligatorio.");

            RuleFor(x => x.ServicioAfectado)
                .NotEmpty().WithMessage("El servicio afectado es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre del servicio afectado no puede superar los 150 caracteres.");

            RuleFor(x => x.DescripcionAmenaza)
                .NotEmpty().WithMessage("La descripción de la amenaza es obligatoria.")
                .MaximumLength(1000).WithMessage("La descripción no puede superar los 1000 caracteres.");

            RuleFor(x => x.NivelDeImpactoValor)
                .InclusiveBetween(1, 4).WithMessage("El nivel de impacto de la amenaza debe ser válido (1=Baja, 2=Media, 3=Alta, 4=Crítica).");

            RuleFor(x => x.PlanDeMitigacion)
                .NotEmpty().WithMessage("El plan de mitigación es obligatorio.")
                .MaximumLength(2000).WithMessage("El plan de mitigación no puede superar los 2000 caracteres.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El identificador del usuario que registra el riesgo es obligatorio.");
        }
    }
}
