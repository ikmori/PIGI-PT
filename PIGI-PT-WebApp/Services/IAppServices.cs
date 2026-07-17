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
        Task<Ticket?> GetTicketByIdAsync(Guid id);
        Task<bool> CancelTicketAsync(Guid id);
        Task<bool> ResolveTicketAsync(Guid id);
        Task<bool> AssignResponsableAsync(Guid id, Guid responsableId);
    }

    public interface IDashboardService
    {
        Task<DashboardMetrics> GetMetricsAsync();
    }

    public interface IAuthService
    {
        Task<Usuario> GetCurrentUserAsync();
        Task<int> GetUnreadNotificationsCountAsync();
        Task<bool> LoginAsync(string email, string password);
        Task LogoutAsync();
        string? GetJwtToken();
    }

    public interface IUsuarioService
    {
        Task<List<Usuario>> GetUsuariosAsync(Guid inquilinoId, bool soloActivos = false);
        Task<Usuario> CreateUsuarioAsync(Usuario usuario, string password);
        Task<Usuario> DesactivarUsuarioAsync(Guid id, Guid adminId);
        Task<Usuario> ReactivarUsuarioAsync(Guid id, Guid adminId);
        Task<Usuario> CambiarRolAsync(Guid id, int nuevoRolValor, Guid modificadorId);
        Task<Usuario> AsignarCategoriaAsync(Guid id, Guid categoriaId, Guid adminId);
        Task<Usuario> DesasignarCategoriaAsync(Guid id, Guid categoriaId, Guid adminId);
    }

    public interface ICategoriaService
    {
        Task<List<Categoria>> GetCategoriasAsync(Guid inquilinoId);
        Task<Categoria> CreateCategoriaAsync(Categoria categoria);
    }

    public interface IRiesgoService
    {
        Task<List<Riesgo>> GetRiesgosAsync(Guid inquilinoId, bool soloUrgentes = false);
        Task<Riesgo> CreateRiesgoAsync(Riesgo riesgo, Guid userId);
        Task<Riesgo> ReevaluarImpactoAsync(Guid id, int nuevoImpactoValor, Guid userId);
    }

    // Modelos auxiliares para el Dashboard
    public class DashboardMetrics
    {
        public int ActiveTickets { get; set; }
        public int CriticalRisks { get; set; }
        public double AiAccuracy { get; set; }
        public double ResolutionTimeHours { get; set; }
        public List<int> WeeklyIncidentVolumes { get; set; } = new List<int> { 0, 0, 0, 0, 0 };
    }
}
