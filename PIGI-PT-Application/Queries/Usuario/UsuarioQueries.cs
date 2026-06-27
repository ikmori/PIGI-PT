using MediatR;
using PIGI_PT_Application.Commands.Usuario;
using PIGI_PT_Application.DTOs.Usuario;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.Specifications.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Queries.Usuario
{
    public class GetUsuarioByIdQuery : IRequest<UsuarioDto>
    {
        public Guid UsuarioId { get; set; }
    }

    public class GetUsuarioByIdQueryHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUsuarioByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(request.UsuarioId)
                ?? throw new KeyNotFoundException($"Usuario con ID '{request.UsuarioId}' no encontrado.");

            return UsuarioMapper.ToDto(usuario);
        }
    }

    public class GetUsuariosByInquilinoQuery : IRequest<List<UsuarioDto>>
    {
        public Guid InquilinoId { get; set; }
        public bool SoloActivos { get; set; } = true;
    }

    public class GetUsuariosByInquilinoQueryHandler : IRequestHandler<GetUsuariosByInquilinoQuery, List<UsuarioDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUsuariosByInquilinoQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UsuarioDto>> Handle(GetUsuariosByInquilinoQuery request, CancellationToken cancellationToken)
        {
            List<PIGI_PT_Domain.Aggregates.Usuario.Usuario> usuarios;

            if (request.SoloActivos)
            {
                var spec = new ActiveUsuariosByInquilinoSpec(request.InquilinoId);
                usuarios = await _unitOfWork.Usuarios.GetBySpecificationAsync(spec);
            }
            else
            {
                var spec = new UsuariosByInquilinoSpec(request.InquilinoId);
                usuarios = await _unitOfWork.Usuarios.GetBySpecificationAsync(spec);
            }

            return usuarios.Select(UsuarioMapper.ToDto).ToList();
        }
    }
}
