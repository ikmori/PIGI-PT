using MediatR;
using PIGI_PT_Application.DTOs.Usuario;
using PIGI_PT_Application.Ports.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PIGI_PT_Application.Commands.Usuario
{
    /// <summary>
    /// Command para asignar una categoría a un operador.
    /// Define el área de responsabilidad del operador.
    /// Solo Admin/SuperAdmin pueden ejecutar este comando.
    /// </summary>
    public class AssignCategoriaToOperadorCommand : IRequest<UsuarioDto>
    {
        public Guid OperadorId { get; set; }
        public Guid CategoriaId { get; set; }
        public Guid AdminId { get; set; }
    }

    public class AssignCategoriaToOperadorCommandHandler : IRequestHandler<AssignCategoriaToOperadorCommand, UsuarioDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignCategoriaToOperadorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Handle(AssignCategoriaToOperadorCommand request, CancellationToken cancellationToken)
        {
            var operador = await _unitOfWork.Usuarios.GetByIdAsync(request.OperadorId)
                ?? throw new KeyNotFoundException($"Operador con ID '{request.OperadorId}' no encontrado.");

            // Validar que la categoría exista
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(request.CategoriaId)
                ?? throw new KeyNotFoundException($"Categoría con ID '{request.CategoriaId}' no encontrada.");

            // Delegar al dominio (valida que sea operador, activo, etc.)
            operador.AsignarDepartamento(request.CategoriaId, request.AdminId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UsuarioMapper.ToDto(operador);
        }
    }

    /// <summary>
    /// Command para desasignar una categoría de un operador.
    /// </summary>
    public class RemoveCategoriaFromOperadorCommand : IRequest<UsuarioDto>
    {
        public Guid OperadorId { get; set; }
        public Guid CategoriaId { get; set; }
        public Guid AdminId { get; set; }
    }

    public class RemoveCategoriaFromOperadorCommandHandler : IRequestHandler<RemoveCategoriaFromOperadorCommand, UsuarioDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveCategoriaFromOperadorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Handle(RemoveCategoriaFromOperadorCommand request, CancellationToken cancellationToken)
        {
            var operador = await _unitOfWork.Usuarios.GetByIdAsync(request.OperadorId)
                ?? throw new KeyNotFoundException($"Operador con ID '{request.OperadorId}' no encontrado.");

            operador.DesasignarDepartamento(request.AdminId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UsuarioMapper.ToDto(operador);
        }
    }

    /// <summary>
    /// Command para cambiar el rol de un usuario existente.
    /// Solo Admin/SuperAdmin pueden ejecutar este comando.
    /// </summary>
    public class CambiarRolUsuarioCommand : IRequest<UsuarioDto>
    {
        public Guid UsuarioId { get; set; }
        public int NuevoRolValor { get; set; }
        public Guid ModificadorId { get; set; }
    }

    public class CambiarRolUsuarioCommandHandler : IRequestHandler<CambiarRolUsuarioCommand, UsuarioDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CambiarRolUsuarioCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Handle(CambiarRolUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(request.UsuarioId)
                ?? throw new KeyNotFoundException($"Usuario con ID '{request.UsuarioId}' no encontrado.");

            var nuevoRol = PIGI_PT_Domain.ValueObjects.Rol.Create(request.NuevoRolValor);
            usuario.CambiarRol(nuevoRol, request.ModificadorId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return UsuarioMapper.ToDto(usuario);
        }
    }
}
