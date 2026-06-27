using MediatR;
using PIGI_PT_Application.Commands.Categoria;
using PIGI_PT_Application.DTOs.Categoria;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.Specifications.Categoria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Queries.Categoria
{
    public class GetCategoriaByIdQuery : IRequest<CategoriaDto>
    {
        public Guid CategoriaId { get; set; }
    }

    public class GetCategoriaByIdQueryHandler : IRequestHandler<GetCategoriaByIdQuery, CategoriaDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoriaByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoriaDto> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(request.CategoriaId)
                ?? throw new KeyNotFoundException($"Categoría con ID '{request.CategoriaId}' no encontrada.");

            return CategoriaMapper.ToDto(categoria);
        }
    }

    public class GetCategoriasByInquilinoQuery : IRequest<List<CategoriaDto>>
    {
        public Guid InquilinoId { get; set; }
    }

    public class GetCategoriasByInquilinoQueryHandler : IRequestHandler<GetCategoriasByInquilinoQuery, List<CategoriaDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoriasByInquilinoQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoriaDto>> Handle(GetCategoriasByInquilinoQuery request, CancellationToken cancellationToken)
        {
            var spec = new CategoriasByInquilinoSpec(request.InquilinoId);
            var categorias = await _unitOfWork.Categorias.GetBySpecificationAsync(spec);

            return categorias.Select(CategoriaMapper.ToDto).ToList();
        }
    }
}
