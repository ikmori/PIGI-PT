using System;
using System.Collections.Generic;

namespace PIGI_PT_WebApp.Models
{
    // ENUMS ALINEADOS A LA FASE 1
    public enum EstadoTicket
    {
        Nuevo = 1,
        AnalizadoPorIA = 2,
        EnProgreso = 3,
        Resuelto = 4,
        Cerrado = 5
    }

    public enum NivelPrioridad
    {
        Baja = 1,
        Media = 2,
        Alta = 3,
        Critica = 4
    }

    public enum Rol
    {
        SuperAdmin = 1,
        Admin = 2,
        Operador = 3,
        UsuarioGeneral = 4
    }

    // CLASES MOCK
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Rol Rol { get; set; } = Rol.UsuarioGeneral;
        public Guid InquilinoId { get; set; }
    }

    public class Categoria
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }

    public class Ticket
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Codigo { get; set; } = $"TKT-{new Random().Next(1000, 9999)}";
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaResolucion { get; set; }
        
        public EstadoTicket Estado { get; set; } = EstadoTicket.Nuevo;
        public NivelPrioridad Prioridad { get; set; } = NivelPrioridad.Media;
        
        public Guid CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
        
        public Guid CreadorId { get; set; }
        public string CreadorNombre { get; set; } = "Usuario Actual";

        public Guid? AsignadoAId { get; set; }
        public string AsignadoANombre { get; set; } = string.Empty;

        // Propiedad mock para la IA
        public bool Sanitizado { get; set; } = false;
        public string SugerenciaIA { get; set; } = string.Empty;
    }

    // DATOS DE PRUEBA
    public static class MockData
    {
        public static List<Ticket> GetTickets()
        {
            return new List<Ticket>
            {
                new Ticket {
                    Titulo = "Error de conexión VPN",
                    Descripcion = "No puedo acceder a la VPN desde mi casa.",
                    Estado = EstadoTicket.EnProgreso,
                    Prioridad = NivelPrioridad.Alta,
                    AsignadoANombre = "Carlos Operador",
                    FechaCreacion = DateTime.Now.AddHours(-2),
                    Sanitizado = true,
                    SugerenciaIA = "Revisar logs del cliente VPN."
                },
                new Ticket {
                    Titulo = "Restablecimiento de contraseña",
                    Descripcion = "Olvidé mi contraseña del portal de nómina.",
                    Estado = EstadoTicket.Nuevo,
                    Prioridad = NivelPrioridad.Baja,
                    FechaCreacion = DateTime.Now.AddMinutes(-30)
                },
                new Ticket {
                    Titulo = "Caída del servidor de base de datos",
                    Descripcion = "El servidor principal de BD en producción no responde.",
                    Estado = EstadoTicket.Resuelto,
                    Prioridad = NivelPrioridad.Critica,
                    AsignadoANombre = "Admin Sistema",
                    FechaCreacion = DateTime.Now.AddDays(-1),
                    FechaResolucion = DateTime.Now.AddHours(-10),
                    Sanitizado = true
                },
                new Ticket {
                    Titulo = "Lentitud en la red",
                    Descripcion = "La red del piso 4 está muy lenta.",
                    Estado = EstadoTicket.AnalizadoPorIA,
                    Prioridad = NivelPrioridad.Media,
                    FechaCreacion = DateTime.Now.AddHours(-5),
                    Sanitizado = true,
                    SugerenciaIA = "Posible congestión de switch. Escalar a redes."
                }
            };
        }
    }
}
