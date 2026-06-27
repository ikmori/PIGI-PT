using FluentValidation;
using PIGI_PT_Application.Commands.Categoria;

namespace PIGI_PT_Application.Validations.Categoria
{
    public class CreateCategoriaCommandValidator : AbstractValidator<CreateCategoriaCommand>
    {
        public CreateCategoriaCommandValidator()
        {
            RuleFor(x => x.NombreCategoria)
                .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

            RuleFor(x => x.InquilinoId)
                .NotEmpty().WithMessage("El identificador del inquilino es obligatorio.");
        }
    }
}
