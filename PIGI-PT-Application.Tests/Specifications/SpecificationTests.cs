using PIGI_PT_Domain.Aggregates.Inquilino;
using PIGI_PT_Domain.Aggregates.Ticket;
using PIGI_PT_Domain.Aggregates.Usuario;
using PIGI_PT_Domain.Specifications.Inquilino;
using PIGI_PT_Domain.Specifications.Ticket;
using PIGI_PT_Domain.Specifications.Usuario;
using PIGI_PT_Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PIGI_PT_Application.Tests.Specifications
{
    public class SpecificationTests
    {
        [Fact]
        public void ActiveTicketsSpec_ShouldFilterCorrectly()
        {
            // Arrange
            var inquilinoId = Guid.NewGuid();
            var ticketActivo1 = new Ticket(inquilinoId, "Ticket Activo 1", "Descripcion 1", Guid.NewGuid());
            var ticketActivo2 = new Ticket(inquilinoId, "Ticket Activo 2", "Descripcion 2", Guid.NewGuid());
            
            var ticketResuelto = new Ticket(inquilinoId, "Ticket Resuelto", "Descripcion 3", Guid.NewGuid());
            ticketResuelto.ClasificarPorIA(NivelPrioridad.Media, Guid.NewGuid());
            ticketResuelto.AsignarOperador(Guid.NewGuid(), Guid.NewGuid());
            ticketResuelto.Resolver(Guid.NewGuid()); // Cambia estado a Resuelto

            var list = new List<Ticket> { ticketActivo1, ticketActivo2, ticketResuelto };
            var spec = new ActiveTicketsSpec(inquilinoId);
            var compiledCriteria = spec.Criteria.Compile();

            // Act
            var filtered = list.Where(compiledCriteria).ToList();

            // Assert
            Assert.Equal(2, filtered.Count);
            Assert.Contains(ticketActivo1, filtered);
            Assert.Contains(ticketActivo2, filtered);
            Assert.DoesNotContain(ticketResuelto, filtered);
        }

        [Fact]
        public void ActiveInquilinosSpec_ShouldFilterActiveInquilinosOnly()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var inquilinoActivo = new Inquilino("Empresa A", "empresa-a.com", adminId);
            var inquilinoSuspendido = new Inquilino("Empresa B", "empresa-b.com", adminId);
            inquilinoSuspendido.SuspenderServicio(adminId, "Falta de pago");

            // Usar reflexión para simular desactivación (IsActive = false) ya que el setter es protected
            var prop = typeof(Inquilino).GetProperty("IsActive", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prop != null)
            {
                prop.SetValue(inquilinoSuspendido, false);
            }

            var list = new List<Inquilino> { inquilinoActivo, inquilinoSuspendido };
            var spec = new ActiveInquilinosSpec();
            var compiledCriteria = spec.Criteria.Compile();

            // Act
            var filtered = list.Where(compiledCriteria).ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Contains(inquilinoActivo, filtered);
            Assert.DoesNotContain(inquilinoSuspendido, filtered);
        }

        [Fact]
        public void UsuariosByInquilinoSpec_ShouldFilterByInquilinoId()
        {
            // Arrange
            var inquilino1Id = Guid.NewGuid();
            var inquilino2Id = Guid.NewGuid();
            var userInquilino1 = new Usuario(inquilino1Id, "User 1", "user1@test.com", "user1", "hashpwd", Rol.UsuarioGeneral);
            var userInquilino2 = new Usuario(inquilino2Id, "User 2", "user2@test.com", "user2", "hashpwd", Rol.Operador);

            var list = new List<Usuario> { userInquilino1, userInquilino2 };
            var spec = new UsuariosByInquilinoSpec(inquilino1Id);
            var compiledCriteria = spec.Criteria.Compile();

            // Act
            var filtered = list.Where(compiledCriteria).ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Contains(userInquilino1, filtered);
            Assert.DoesNotContain(userInquilino2, filtered);
        }
    }
}
