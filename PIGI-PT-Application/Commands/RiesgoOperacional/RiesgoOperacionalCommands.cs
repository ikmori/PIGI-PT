using MediatR;
using PIGI_PT_Application.DTOs.RiesgoOperacional;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Commands.RiesgoOperacional
{
    public class CreateRiesgoOperacionalCommand : IRequest<RiesgoOperacionalDto>
    {
        public Guid InquilinoId { get; set; }
        public string ServicioAfectado { get; set; } = string.Empty;
        public string DescripcionAmenaza { get; set; } = string.Empty;
        public int NivelDeImpactoValor { get; set; }
        public string PlanDeMitigacion { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class CreateRiesgoOperacionalCommandHandler : IRequestHandler<CreateRiesgoOperacionalCommand, RiesgoOperacionalDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRiesgoOperacionalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RiesgoOperacionalDto> Handle(CreateRiesgoOperacionalCommand request, CancellationToken cancellationToken)
        {
            var nivelImpacto = NivelPrioridad.Create(request.NivelDeImpactoValor);

            var riesgo = new PIGI_PT_Domain.Aggregates.RiesgoOperacional.RiesgoOperacional(
                request.InquilinoId,
                request.ServicioAfectado,
                request.DescripcionAmenaza,
                nivelImpacto,
                request.PlanDeMitigacion,
                request.UserId
            );

            await _unitOfWork.RiesgosOperacionales.AddAsync(riesgo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RiesgoOperacionalMapper.ToDto(riesgo);
        }
    }

    public class ReevaluarImpactoRiesgoCommand : IRequest<RiesgoOperacionalDto>
    {
        public Guid RiesgoId { get; set; }
        public int NuevoImpactoValor { get; set; }
        public Guid UserId { get; set; }
    }

    public class ReevaluarImpactoRiesgoCommandHandler : IRequestHandler<ReevaluarImpactoRiesgoCommand, RiesgoOperacionalDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReevaluarImpactoRiesgoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RiesgoOperacionalDto> Handle(ReevaluarImpactoRiesgoCommand request, CancellationToken cancellationToken)
        {
            var riesgo = await _unitOfWork.RiesgosOperacionales.GetByIdAsync(request.RiesgoId)
                ?? throw new KeyNotFoundException($"Riesgo operacional con ID '{request.RiesgoId}' no encontrado.");

            var nuevoImpacto = NivelPrioridad.Create(request.NuevoImpactoValor);

            riesgo.ReevaluarImpacto(nuevoImpacto, request.UserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RiesgoOperacionalMapper.ToDto(riesgo);
        }
    }
}
