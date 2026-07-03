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

            // 2. Crear Usuarios ligados al Inquilino con diferentes roles
            // Usamos password123 como contraseña mock
            var adminUser = new Usuario(
                inquilino.Id,
                "Admin PIGI",
                "admin@empresa.com",
                "admin",
                "password123",
                Rol.SuperAdmin
            );
            
            var mariaUser = new Usuario(
                inquilino.Id,
                "María García",
                "maria@empresa.com",
                "maria",
                "password123",
                Rol.Admin
            );
            
            var carlosUser = new Usuario(
                inquilino.Id,
                "Carlos Operador",
                "carlos@empresa.com",
                "carlos",
                "password123",
                Rol.Operador
            );
            
            var anaUser = new Usuario(
                inquilino.Id,
                "Ana López",
                "ana@empresa.com",
                "ana",
                "password123",
                Rol.Operador
            );
            
            var pedroUser = new Usuario(
                inquilino.Id,
                "Pedro Martínez",
                "pedro@empresa.com",
                "pedro",
                "password123",
                Rol.UsuarioGeneral
            );

            context.Usuarios.AddRange(adminUser, mariaUser, carlosUser, anaUser, pedroUser);

            // 3. Crear Categorías por defecto
            var catHardware = new Categoria("Hardware", "Problemas físicos con equipos (laptops, pantallas, periféricos)", inquilino.Id);
            var catSoftware = new Categoria("Software/Aplicaciones", "Errores en programas, ERP, correo y suites de oficina", inquilino.Id);
            var catRedes = new Categoria("Redes y Conectividad", "Fallas de red local, internet corporativo y acceso VPN", inquilino.Id);
            var catAccesos = new Categoria("Accesos y Contraseñas", "Gestión de cuentas, perfiles de usuario y credenciales", inquilino.Id);

            context.Categorias.AddRange(catHardware, catSoftware, catRedes, catAccesos);

            // Asignar categorías a los operadores para definir su área de responsabilidad
            carlosUser.AsignarCategoria(catRedes.Id, adminUser.Id);
            carlosUser.AsignarCategoria(catHardware.Id, adminUser.Id);
            anaUser.AsignarCategoria(catSoftware.Id, adminUser.Id);
            anaUser.AsignarCategoria(catAccesos.Id, adminUser.Id);

            // Guardar usuarios y categorías
            await context.SaveChangesAsync();

            // 4. Crear Tickets iniciales
            // Ticket 1: En progreso, asignado a Carlos (Redes)
            var ticket1 = new Ticket(inquilino.Id, "Error de conexión VPN", "No puedo acceder a la VPN desde mi casa.", pedroUser.Id);
            ticket1.AplicarSanitizacion("No puedo acceder a la VPN desde mi casa.");
            ticket1.ClasificarPorIA(NivelPrioridad.Alta, catRedes.Id);
            ticket1.AsignarOperador(carlosUser.Id, adminUser.Id);

            // Ticket 2: Nuevo (Pendiente de análisis) creado por Pedro
            var ticket2 = new Ticket(inquilino.Id, "Restablecimiento de contraseña", "Olvidé mi contraseña del portal de nómina.", pedroUser.Id);

            // Ticket 3: Resuelto por Ana (Software)
            var ticket3 = new Ticket(inquilino.Id, "Caída del servidor de base de datos", "El servidor principal de BD en producción no responde.", pedroUser.Id);
            ticket3.AplicarSanitizacion("El servidor principal de BD en producción no responde.");
            ticket3.ClasificarPorIA(NivelPrioridad.Critica, catSoftware.Id);
            ticket3.AsignarOperador(anaUser.Id, adminUser.Id);
            ticket3.Resolver(anaUser.Id);

            // Ticket 4: Analizado por IA (Hardware)
            var ticket4 = new Ticket(inquilino.Id, "Lentitud en la red", "La red del piso 4 está muy lenta.", pedroUser.Id);
            ticket4.AplicarSanitizacion("La red del piso 4 está muy lenta.");
            ticket4.ClasificarPorIA(NivelPrioridad.Media, catRedes.Id);

            context.Tickets.AddRange(ticket1, ticket2, ticket3, ticket4);

            // Guardar tickets finales
            await context.SaveChangesAsync();
        }
    }
}
