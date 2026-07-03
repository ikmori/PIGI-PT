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
        public bool IsActive { get; set; } = true;
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
    public class Riesgo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InquilinoId { get; set; }
        public string ServicioAfectado { get; set; } = string.Empty;
        public string Amenaza { get; set; } = string.Empty;
        public string Impacto { get; set; } = string.Empty;
        public int ImpactoValor { get; set; }
        public string PlanMitigacion { get; set; } = string.Empty;
        public DateTime UltimaRevision { get; set; }
        public bool RequiereRevision { get; set; }
    }


}
