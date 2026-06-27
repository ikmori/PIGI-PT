using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PIGI_PT_WebApp.Models;

namespace PIGI_PT_WebApp.Services
{
    public class MockTicketService : ITicketService
    {
        public Task<List<Ticket>> GetTicketsAsync()
        {
            return Task.FromResult(MockData.GetTickets());
        }

        public Task<List<Ticket>> GetHighPriorityTicketsAsync(int count)
        {
            var tickets = MockData.GetTickets()
                .Where(t => t.Prioridad >= NivelPrioridad.Alta)
                .Take(count)
                .ToList();
            return Task.FromResult(tickets);
        }

        public Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            // Simular creación
            ticket.Id = System.Guid.NewGuid();
            ticket.Codigo = $"TKT-{new System.Random().Next(1000, 9999)}";
            ticket.Estado = EstadoTicket.Nuevo;
            return Task.FromResult(ticket);
        }
    }

    public class MockDashboardService : IDashboardService
    {
        public Task<DashboardMetrics> GetMetricsAsync()
        {
            return Task.FromResult(new DashboardMetrics
            {
                ActiveTickets = 24,
                CriticalRisks = 3,
                AiAccuracy = 94.2,
                ResolutionTimeHours = 4.5
            });
        }
    }

    public class MockAuthService : IAuthService
    {
        public Task<Usuario> GetCurrentUserAsync()
        {
            return Task.FromResult(new Usuario
            {
                Nombre = "Admin PIGI",
                Rol = Rol.SuperAdmin,
                Email = "admin@empresa.com"
            });
        }

        public Task<int> GetUnreadNotificationsCountAsync()
        {
            return Task.FromResult(3);
        }

        public Task<bool> LoginAsync(string email, string password)
        {
            return Task.FromResult(email == "admin@empresa.com" && password == "password123");
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }
    }
}
