using MediatR;
using PIGI_PT_Application.Commands.Inquilino;
using PIGI_PT_Application.DTOs.Inquilino;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.Specifications.Inquilino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Queries.Inquilino
{
    public class GetInquilinoByIdQuery : IRequest<InquilinoDto>
    {
        public Guid InquilinoId { get; set; }
    }

    public class GetInquilinoByIdQueryHandler : IRequestHandler<GetInquilinoByIdQuery, InquilinoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetInquilinoByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InquilinoDto> Handle(GetInquilinoByIdQuery request, CancellationToken cancellationToken)
        {
            var inquilino = await _unitOfWork.Inquilinos.GetByIdAsync(request.InquilinoId)
                ?? throw new KeyNotFoundException($"Inquilino con ID '{request.InquilinoId}' no encontrado.");

            return InquilinoMapper.ToDto(inquilino);
        }
    }

    public class GetInquilinosQuery : IRequest<List<InquilinoDto>>
    {
        public bool SoloActivos { get; set; } = true;
    }

    public class GetInquilinosQueryHandler : IRequestHandler<GetInquilinosQuery, List<InquilinoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetInquilinosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<InquilinoDto>> Handle(GetInquilinosQuery request, CancellationToken cancellationToken)
        {
            List<PIGI_PT_Domain.Aggregates.Inquilino.Inquilino> inquilinos;

            if (request.SoloActivos)
            {
                var spec = new ActiveInquilinosSpec();
                inquilinos = await _unitOfWork.Inquilinos.GetBySpecificationAsync(spec);
            }
            else
            {
                // Si no se pide solo activos, podemos usar una especificación vacía para traer todos
                // o crear una especificación que traiga todo.
                // Como GetBySpecificationAsync requiere una Spec, podemos crear una genérica
                // o simplemente usar ActiveInquilinosSpec y si es false usar otra.
                // Creemos una especificación rápida o usemo GetBySpecificationAsync con una spec que admita todo.
                var spec = new AllInquilinosSpec();
                inquilinos = await _unitOfWork.Inquilinos.GetBySpecificationAsync(spec);
            }

            return inquilinos.Select(InquilinoMapper.ToDto).ToList();
        }
    }

    public class AllInquilinosSpec : PIGI_PT_Domain.Specifications.Specification<PIGI_PT_Domain.Aggregates.Inquilino.Inquilino>
    {
        public AllInquilinosSpec()
        {
            Criteria = i => true;
            OrderBy = i => i.NombreComercial;
        }
    }
}
