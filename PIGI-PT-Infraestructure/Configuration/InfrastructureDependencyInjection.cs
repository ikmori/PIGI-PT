using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Application.Ports.Repositories;
using PIGI_PT_Infraestructure.Persistence.DbContext;
using PIGI_PT_Infraestructure.Persistence.Repositories;

namespace PIGI_PT_Infraestructure.Configuration
{
    /// <summary>
    /// Extensión de IServiceCollection para registrar todos los servicios
    /// de la capa de Infraestructura: DbContext, repositorios y UnitOfWork.
    /// </summary>
    public static class InfrastructureDependencyInjection
    {
        /// <summary>
        /// Registra los servicios de infraestructura en el contenedor de DI.
        /// </summary>
        /// <param name="services">Colección de servicios de la aplicación.</param>
        /// <param name="configuration">Configuración de la aplicación (appsettings).</param>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── DbContext ────────────────────────────────────────────────────────────
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback: In-Memory database para desarrollo sin SQL Server
                services.AddDbContext<PigiPtDbContext>(options =>
                    options.UseInMemoryDatabase("PigiPtDb"));
            }
            else
            {
                services.AddDbContext<PigiPtDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            // ── Repositorios ─────────────────────────────────────────────────────────
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IInquilinoRepository, InquilinoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IRiesgoOperacionalRepository, RiesgoOperacionalRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            // ── Unit of Work ─────────────────────────────────────────────────────────
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
