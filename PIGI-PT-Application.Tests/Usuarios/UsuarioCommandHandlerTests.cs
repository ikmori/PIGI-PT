using Microsoft.EntityFrameworkCore;
using PIGI_PT_Application.Commands.Usuario;
using PIGI_PT_Domain.ValueObjects;
using PIGI_PT_Infraestructure.Persistence.DbContext;
using PIGI_PT_Infraestructure.Persistence.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PIGI_PT_Application.Tests.Usuarios
{
    /// <summary>
    /// Pruebas unitarias para los Handlers de Comandos del agregado Usuario.
    /// </summary>
    public class UsuarioCommandHandlerTests
    {
        private PigiPtDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<PigiPtDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new PigiPtDbContext(options);
        }

        private UnitOfWork CreateUnitOfWork(PigiPtDbContext context)
        {
            var ticketsRepo = new TicketRepository(context);
            var inquilinosRepo = new InquilinoRepository(context);
            var usuariosRepo = new UsuarioRepository(context);
            var riesgosRepo = new RiesgoOperacionalRepository(context);
            var categoriasRepo = new CategoriaRepository(context);

            return new UnitOfWork(context, ticketsRepo, inquilinosRepo, usuariosRepo, riesgosRepo, categoriasRepo);
        }

        [Fact]
        public async Task CreateUsuarioCommandHandler_ShouldCreateUsuarioAndSaveToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var unitOfWork = CreateUnitOfWork(context);
            var handler = new CreateUsuarioCommandHandler(unitOfWork);

            var inquilinoId = Guid.NewGuid();
            var command = new CreateUsuarioCommand
            {
                InquilinoId = inquilinoId,
                FullName = "Carlos Pérez",
                Email = "carlos@empresa.com",
                UserName = "cperez",
                Password = "hashedpassword",
                RolValor = Rol.OPERADOR_VALUE
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Carlos Pérez", result.FullName);
            Assert.Equal("carlos@empresa.com", result.Email);
            Assert.Equal("Operador", result.Rol);
            Assert.True(result.IsActive);

            var dbUser = await context.Usuarios.FindAsync(result.Id);
            Assert.NotNull(dbUser);
            Assert.Equal("cperez", dbUser.UserName);
        }

        [Fact]
        public async Task DesactivarUsuarioCommandHandler_ShouldDeactivateUsuarioCorrectly()
        {
            // Arrange
            using var context = CreateDbContext();
            var unitOfWork = CreateUnitOfWork(context);
            
            var inquilinoId = Guid.NewGuid();
            var usuario = new PIGI_PT_Domain.Aggregates.Usuario.Usuario(
                inquilinoId,
                "Ana Gómez",
                "ana@empresa.com",
                "agomez",
                "pass123",
                Rol.UsuarioGeneral
            );

            await context.Usuarios.AddAsync(usuario);
            await context.SaveChangesAsync();

            var handler = new DesactivarUsuarioCommandHandler(unitOfWork);
            var command = new DesactivarUsuarioCommand
            {
                UsuarioId = usuario.Id,
                ModificadorId = Guid.NewGuid()
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsActive);

            var dbUser = await context.Usuarios.FindAsync(usuario.Id);
            Assert.NotNull(dbUser);
            Assert.False(dbUser.IsActive);
        }
    }
}
