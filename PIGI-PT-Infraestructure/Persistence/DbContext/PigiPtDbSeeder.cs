using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PIGI_PT_Domain.Aggregates.Categoria;
using PIGI_PT_Domain.Aggregates.Inquilino;
using PIGI_PT_Domain.Aggregates.Ticket;
using PIGI_PT_Domain.Aggregates.Usuario;
using PIGI_PT_Domain.ValueObjects;

namespace PIGI_PT_Infraestructure.Persistence.DbContext
{
    /// <summary>
    /// Sembrador de datos iniciales para el entorno de desarrollo y pruebas.
    /// Crea un Inquilino, un Usuario Administrador, Categorías y Tickets base.
    /// </summary>
    public static class PigiPtDbSeeder
    {
        public static async Task SeedAsync(PigiPtDbContext context)
        {
            // Validar si ya hay inquilinos en el sistema. Si existen, asumimos que ya está sembrado.
            if (await context.Inquilinos.AnyAsync())
            {
                return;
            }

            // 1. Crear Inquilino Demo (Id generado automáticamente en dominio)
            var temporalCreatorId = Guid.NewGuid();
            var inquilino = new Inquilino("Inquilino Demo", "empresa.com", temporalCreatorId);
            context.Inquilinos.Add(inquilino);

            // Guardar inquilino para obtener su Id generado
            await context.SaveChangesAsync();

            // 2. Crear Usuario Administrador ligado al Inquilino
            // Usamos password123 como contraseña hasheada mock
            var adminUser = new Usuario(
                inquilino.Id,
                "Admin PIGI",
                "admin@empresa.com",
                "admin",
                "password123",
                Rol.SuperAdmin
            );
            context.Usuarios.Add(adminUser);

            // 3. Crear Categorías por defecto
            var catHardware = new Categoria("Hardware", "Problemas físicos con equipos (laptops, pantallas, periféricos)", inquilino.Id);
            var catSoftware = new Categoria("Software/Aplicaciones", "Errores en programas, ERP, correo y suites de oficina", inquilino.Id);
            var catRedes = new Categoria("Redes y Conectividad", "Fallas de red local, internet corporativo y acceso VPN", inquilino.Id);
            var catAccesos = new Categoria("Accesos y Contraseñas", "Gestión de cuentas, perfiles de usuario y credenciales", inquilino.Id);

            context.Categorias.AddRange(catHardware, catSoftware, catRedes, catAccesos);

            // Guardar usuarios y categorías
            await context.SaveChangesAsync();

            // 4. Crear Tickets iniciales
            // Ticket 1: En progreso
            var ticket1 = new Ticket(inquilino.Id, "Error de conexión VPN", "No puedo acceder a la VPN desde mi casa.", adminUser.Id);
            ticket1.AplicarSanitizacion("No puedo acceder a la VPN desde mi casa.");
            ticket1.ClasificarPorIA(NivelPrioridad.Alta, catRedes.Id);
            ticket1.AsignarOperador(adminUser.Id, adminUser.Id);

            // Ticket 2: Nuevo (Pendiente de análisis)
            var ticket2 = new Ticket(inquilino.Id, "Restablecimiento de contraseña", "Olvidé mi contraseña del portal de nómina.", adminUser.Id);

            // Ticket 3: Resuelto
            var ticket3 = new Ticket(inquilino.Id, "Caída del servidor de base de datos", "El servidor principal de BD en producción no responde.", adminUser.Id);
            ticket3.AplicarSanitizacion("El servidor principal de BD en producción no responde.");
            ticket3.ClasificarPorIA(NivelPrioridad.Critica, catSoftware.Id);
            ticket3.AsignarOperador(adminUser.Id, adminUser.Id);
            ticket3.Resolver(adminUser.Id);

            // Ticket 4: Analizado por IA
            var ticket4 = new Ticket(inquilino.Id, "Lentitud en la red", "La red del piso 4 está muy lenta.", adminUser.Id);
            ticket4.AplicarSanitizacion("La red del piso 4 está muy lenta.");
            ticket4.ClasificarPorIA(NivelPrioridad.Media, catRedes.Id);

            context.Tickets.AddRange(ticket1, ticket2, ticket3, ticket4);

            // Guardar tickets finales
            await context.SaveChangesAsync();
        }
    }
}
