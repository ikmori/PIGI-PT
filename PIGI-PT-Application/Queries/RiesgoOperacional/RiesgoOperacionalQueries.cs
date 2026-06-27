using MediatR;
using PIGI_PT_Application.Commands.RiesgoOperacional;
using PIGI_PT_Application.DTOs.RiesgoOperacional;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.Specifications.RiesgoOperacional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Queries.RiesgoOperacional
{
    public class GetRiesgoByIdQuery : IRequest<RiesgoOperacionalDto>
    {
        public Guid RiesgoId { get; set; }
    }

    public class GetRiesgoByIdQueryHandler : IRequestHandler<GetRiesgoByIdQuery, RiesgoOperacionalDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRiesgoByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RiesgoOperacionalDto> Handle(GetRiesgoByIdQuery request, CancellationToken cancellationToken)
        {
            var riesgo = await _unitOfWork.RiesgosOperacionales.GetByIdAsync(request.RiesgoId)
                ?? throw new KeyNotFoundException($"Riesgo operacional con ID '{request.RiesgoId}' no encontrado.");

            return RiesgoOperacionalMapper.ToDto(riesgo);
        }
    }

    public class GetRiesgosByInquilinoQuery : IRequest<List<RiesgoOperacionalDto>>
    {
        public Guid InquilinoId { get; set; }
        public bool SoloRevisionUrgente { get; set; } = false;
    }

    public class GetRiesgosByInquilinoQueryHandler : IRequestHandler<GetRiesgosByInquilinoQuery, List<RiesgoOperacionalDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRiesgosByInquilinoQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RiesgoOperacionalDto>> Handle(GetRiesgosByInquilinoQuery request, CancellationToken cancellationToken)
        {
            List<PIGI_PT_Domain.Aggregates.RiesgoOperacional.RiesgoOperacional> riesgos;

            if (request.SoloRevisionUrgente)
            {
                var spec = new RiesgosRequierenRevisionSpec(request.InquilinoId);
                riesgos = await _unitOfWork.RiesgosOperacionales.GetBySpecificationAsync(spec);
            }
            else
            {
                var spec = new RiesgosByInquilinoSpec(request.InquilinoId);
                riesgos = await _unitOfWork.RiesgosOperacionales.GetBySpecificationAsync(spec);
            }

            return riesgos.Select(RiesgoOperacionalMapper.ToDto).ToList();
        }
    }
}
