using Microsoft.EntityFrameworkCore;
using PIGI_PT_Application.Commands.Inquilino;
using PIGI_PT_Application.Commands.Ticket;
using PIGI_PT_Application.Commands.Usuario;
using PIGI_PT_Domain.Aggregates.Inquilino;
using PIGI_PT_Domain.Aggregates.Ticket;
using PIGI_PT_Domain.ValueObjects;
using PIGI_PT_Infraestructure.Persistence.DbContext;
using PIGI_PT_Infraestructure.Persistence.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PIGI_PT_Application.Tests.Tickets
{
    public class TicketCommandHandlerTests
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
        public async Task CreateTicketCommandHandler_ShouldCreateTicketAndSaveToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var unitOfWork = CreateUnitOfWork(context);
            var handler = new CreateTicketCommandHandler(unitOfWork);

            var inquilinoId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CreateTicketCommand
            {
                InquilinoId = inquilinoId,
                Titulo = "Servidor caído",
                Descripcion = "El servidor principal de base de datos no responde.",
                UserId = userId
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Servidor caído", result.Titulo);
            Assert.Equal("Pendiente de Análisis", result.Estado); // Estado por defecto en constructor de Ticket
            Assert.Equal(inquilinoId, result.InquilinoId);
 
            var dbTicket = await context.Tickets.FindAsync(result.Id);
            Assert.NotNull(dbTicket);
            Assert.Equal("Servidor caído", dbTicket.Titulo);
        }
 
        [Fact]
        public async Task ResolveTicketCommandHandler_ShouldResolveTicketCorrectly()
        {
            // Arrange
            using var context = CreateDbContext();
            var unitOfWork = CreateUnitOfWork(context);
            
            var inquilinoId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();
            
            // Crear ticket directamente en BD, clasificarlo y asignarlo a un operador (porque resolver exige operador asignado)
            var ticket = new Ticket(inquilinoId, "Problema red", "No hay conexión externa", creatorId);
            ticket.ClasificarPorIA(NivelPrioridad.Media, Guid.NewGuid()); // Clasificado
            
            var operadorId = Guid.NewGuid();
            ticket.AsignarOperador(operadorId, creatorId); // Asignado / EnProgreso
            
            await context.Tickets.AddAsync(ticket);
            await context.SaveChangesAsync();

            var handler = new ResolveTicketCommandHandler(unitOfWork);
            var command = new ResolveTicketCommand
            {
                TicketId = ticket.Id,
                UserId = operadorId
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Resuelto", result.Estado);
            Assert.NotNull(result.FechaResolucion);

            var dbTicket = await context.Tickets.FindAsync(ticket.Id);
            Assert.NotNull(dbTicket);
            Assert.Equal(EstadoTicket.Resuelto.Valor, dbTicket.Estado.Valor);
        }

        [Fact]
        public async Task CreateInquilinoCommandHandler_ShouldCreateInquilino()
        {
            // Arrange
            using var context = CreateDbContext();
            var unitOfWork = CreateUnitOfWork(context);
            var handler = new CreateInquilinoCommandHandler(unitOfWork);

            var adminId = Guid.NewGuid();
            var command = new CreateInquilinoCommand
            {
                NombreComercial = "Empresa ACME",
                DominioRed = "acme.com",
                UserId = adminId
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Empresa ACME", result.NombreComercial);
            Assert.Equal("Activo", result.Estado);

            var dbInquilino = await context.Inquilinos.FindAsync(result.Id);
            Assert.NotNull(dbInquilino);
            Assert.Equal("acme.com", dbInquilino.DominioRed);
        }
    }
}
