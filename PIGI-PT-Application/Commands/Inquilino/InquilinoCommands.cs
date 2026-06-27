using MediatR;
using PIGI_PT_Application.DTOs.Inquilino;
using PIGI_PT_Application.Ports.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Commands.Inquilino
{
    public class CreateInquilinoCommand : IRequest<InquilinoDto>
    {
        public string NombreComercial { get; set; } = string.Empty;
        public string DominioRed { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class CreateInquilinoCommandHandler : IRequestHandler<CreateInquilinoCommand, InquilinoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateInquilinoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InquilinoDto> Handle(CreateInquilinoCommand request, CancellationToken cancellationToken)
        {
            var inquilino = new PIGI_PT_Domain.Aggregates.Inquilino.Inquilino(
                request.NombreComercial,
                request.DominioRed,
                request.UserId
            );

            await _unitOfWork.Inquilinos.AddAsync(inquilino);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return InquilinoMapper.ToDto(inquilino);
        }
    }

    public class SuspenderInquilinoCommand : IRequest<InquilinoDto>
    {
        public Guid InquilinoId { get; set; }
        public Guid AdminId { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class SuspenderInquilinoCommandHandler : IRequestHandler<SuspenderInquilinoCommand, InquilinoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SuspenderInquilinoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InquilinoDto> Handle(SuspenderInquilinoCommand request, CancellationToken cancellationToken)
        {
            var inquilino = await _unitOfWork.Inquilinos.GetByIdAsync(request.InquilinoId)
                ?? throw new KeyNotFoundException($"Inquilino con ID '{request.InquilinoId}' no encontrado.");

            inquilino.SuspenderServicio(request.AdminId, request.Motivo);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return InquilinoMapper.ToDto(inquilino);
        }
    }

    public class ReactivarInquilinoCommand : IRequest<InquilinoDto>
    {
        public Guid InquilinoId { get; set; }
        public Guid AdminId { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    public class ReactivarInquilinoCommandHandler : IRequestHandler<ReactivarInquilinoCommand, InquilinoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReactivarInquilinoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InquilinoDto> Handle(ReactivarInquilinoCommand request, CancellationToken cancellationToken)
        {
            var inquilino = await _unitOfWork.Inquilinos.GetByIdAsync(request.InquilinoId)
                ?? throw new KeyNotFoundException($"Inquilino con ID '{request.InquilinoId}' no encontrado.");

            inquilino.ReactivarServicio(request.AdminId, request.Motivo);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return InquilinoMapper.ToDto(inquilino);
        }
    }

    public class ActualizarPermisosIACommand : IRequest<InquilinoDto>
    {
        public Guid InquilinoId { get; set; }
        public bool PermitirIA { get; set; }
        public Guid AdminId { get; set; }
    }

    public class ActualizarPermisosIACommandHandler : IRequestHandler<ActualizarPermisosIACommand, InquilinoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarPermisosIACommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InquilinoDto> Handle(ActualizarPermisosIACommand request, CancellationToken cancellationToken)
        {
            var inquilino = await _unitOfWork.Inquilinos.GetByIdAsync(request.InquilinoId)
                ?? throw new KeyNotFoundException($"Inquilino con ID '{request.InquilinoId}' no encontrado.");

            inquilino.ActualizarPermisosIA(request.PermitirIA, request.AdminId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return InquilinoMapper.ToDto(inquilino);
        }
    }
}
