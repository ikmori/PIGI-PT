using System.Collections.Generic;
using System.Threading.Tasks;
using PIGI_PT_WebApp.Models;

namespace PIGI_PT_WebApp.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetTicketsAsync();
        Task<List<Ticket>> GetHighPriorityTicketsAsync(int count);
        Task<Ticket> CreateTicketAsync(Ticket ticket);
    }

    public interface IDashboardService
    {
        Task<DashboardMetrics> GetMetricsAsync();
    }

    public interface IAuthService
    {
        Task<Usuario> GetCurrentUserAsync();
        Task<int> GetUnreadNotificationsCountAsync();
    }

    // Modelos auxiliares para el Dashboard
    public class DashboardMetrics
    {
        public int ActiveTickets { get; set; }
        public int CriticalRisks { get; set; }
        public double AiAccuracy { get; set; }
        public double ResolutionTimeHours { get; set; }
    }
}
