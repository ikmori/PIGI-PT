using MediatR;
using PIGI_PT_Application.DTOs.Usuario;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Commands.Usuario
{
    public class CreateUsuarioCommand : IRequest<UsuarioDto>
    {
        public Guid InquilinoId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Hash o texto plano
        public int RolValor { get; set; }
    }

    public class CreateUsuarioCommandHandler : IRequestHandler<CreateUsuarioCommand, UsuarioDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUsuarioCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var rol = Rol.Create(request.RolValor);

            var usuario = new PIGI_PT_Domain.Aggregates.Usuario.Usuario(
                request.InquilinoId,
                request.FullName,
                request.Email,
                request.UserName,
                request.Password, // Nota: En un entorno de producción, hasheamos la contraseña
                rol
            );

            await _unitOfWork.Usuarios.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UsuarioMapper.ToDto(usuario);
        }
    }

    public class DesactivarUsuarioCommand : IRequest<UsuarioDto>
    {
        public Guid UsuarioId { get; set; }
        public Guid ModificadorId { get; set; }
    }

    public class DesactivarUsuarioCommandHandler : IRequestHandler<DesactivarUsuarioCommand, UsuarioDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DesactivarUsuarioCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Handle(DesactivarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(request.UsuarioId)
                ?? throw new KeyNotFoundException($"Usuario con ID '{request.UsuarioId}' no encontrado.");

            usuario.DesactivarPerfil(request.ModificadorId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UsuarioMapper.ToDto(usuario);
        }
    }

    public class ReactivarUsuarioCommand : IRequest<UsuarioDto>
    {
        public Guid UsuarioId { get; set; }
        public Guid ModificadorId { get; set; }
    }

    public class ReactivarUsuarioCommandHandler : IRequestHandler<ReactivarUsuarioCommand, UsuarioDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReactivarUsuarioCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Handle(ReactivarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(request.UsuarioId)
                ?? throw new KeyNotFoundException($"Usuario con ID '{request.UsuarioId}' no encontrado.");

            usuario.ReactivarPerfil(request.ModificadorId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UsuarioMapper.ToDto(usuario);
        }
    }
}
