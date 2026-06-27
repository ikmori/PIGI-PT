using MediatR;
using PIGI_PT_Application.DTOs.Categoria;
using PIGI_PT_Application.Ports.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Commands.Categoria
{
    public class CreateCategoriaCommand : IRequest<CategoriaDto>
    {
        public string NombreCategoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public Guid InquilinoId { get; set; }
    }

    public class CreateCategoriaCommandHandler : IRequestHandler<CreateCategoriaCommand, CategoriaDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoriaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoriaDto> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken)
        {
            var categoria = new PIGI_PT_Domain.Aggregates.Categoria.Categoria(
                request.NombreCategoria,
                request.Descripcion,
                request.InquilinoId
            );

            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return CategoriaMapper.ToDto(categoria);
        }
    }
}
