using MediatR;
using PIGI_PT_Application.DTOs.Ticket;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Application.Ports.Services;

namespace PIGI_PT_Application.Commands.Ticket
{
    /// <summary>
    /// Command para crear un nuevo ticket de incidente.
    /// </summary>
    public class CreateTicketCommand : IRequest<TicketDto>
    {
        public Guid InquilinoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Handler que procesa el comando CreateTicketCommand.
    /// Crea la entidad Ticket en el dominio y la persiste via UnitOfWork.
    /// </summary>
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataSanitizerService _sanitizerService;
        private readonly ITicketAnalyzerService _aiAnalyzer;
        private readonly INotificationService _notificationService;

        public CreateTicketCommandHandler(
            IUnitOfWork unitOfWork,
            IDataSanitizerService sanitizerService,
            ITicketAnalyzerService aiAnalyzer,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _sanitizerService = sanitizerService;
            _aiAnalyzer = aiAnalyzer;
            _notificationService = notificationService;
        }

        public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            // 1. Crear entidad de dominio
            var ticket = new PIGI_PT_Domain.Aggregates.Ticket.Ticket(
                request.InquilinoId,
                request.Titulo,
                request.Descripcion,
                request.UserId
            );

            // 2. Sanitización de datos sensibles
            var sanitizedDescription = _sanitizerService.SanitizeText(request.Descripcion);
            ticket.AplicarSanitizacion(sanitizedDescription);

            // 3. Obtener categorías reales de la empresa
            var categoriasSpec = new PIGI_PT_Domain.Specifications.Categoria.CategoriasByInquilinoSpec(request.InquilinoId);
            var categorias = await _unitOfWork.Categorias.GetBySpecificationAsync(categoriasSpec);
            
            Guid categoriaId = Guid.Empty;
            PIGI_PT_Domain.Aggregates.Usuario.Usuario? operadorAdecuado = null;
            string? categoriaNombre = null;

            try 
            {
                var categoriasDisponibles = string.Join("\n", categorias.Select(c => $"[ID: {c.Id}] - {c.NombreCategoria}"));

                if (string.IsNullOrWhiteSpace(categoriasDisponibles))
                {
                    categoriasDisponibles = "No hay categorías definidas. Sugiere crear una por defecto.";
                }

                // 4. Analizar con Gemini AI
                var aiResult = await _aiAnalyzer.AnalyzeTicketAsync(request.Titulo, sanitizedDescription, categoriasDisponibles);

                // 4. Clasificar por IA
                var prioridad = PIGI_PT_Domain.ValueObjects.NivelPrioridad.DesdeString(aiResult.PrioridadSugerida);
                
                // Usar un GUID de categoría (en un escenario real, buscaríamos en BD según aiResult.CategoriaSugerida)
                categoriaId = Guid.TryParse(aiResult.CategoriaSugerida, out var parsedGuid) 
                    ? parsedGuid 
                    : Guid.NewGuid();

                ticket.ClasificarPorIA(prioridad, categoriaId);

                // Auto-asignación inteligente basada en la categoría seleccionada por la IA
                var operadoresSpec = new PIGI_PT_Domain.Specifications.Usuario.ActiveUsuariosByInquilinoSpec(request.InquilinoId);
                var usuarios = await _unitOfWork.Usuarios.GetBySpecificationAsync(operadoresSpec);
                operadorAdecuado = usuarios.FirstOrDefault(u => 
                    (u.Rol == PIGI_PT_Domain.ValueObjects.Rol.DepartamentoTecnologia || u.Rol == PIGI_PT_Domain.ValueObjects.Rol.Admin) &&
                    u.DepartamentoId == categoriaId);

                // 5. Enviar notificación al área responsable
                var destinatarioEmail = operadorAdecuado?.Email ?? "soporte@empresa.com";
                var destinatarioNombre = operadorAdecuado?.FullName ?? "Equipo de Soporte";
                categoriaNombre = categorias.FirstOrDefault(c => c.Id == categoriaId)?.NombreCategoria ?? "Sin categoría";

                await _notificationService.SendEmailAsync(
                    destinatarioEmail,
                    $"[PIGI-PT] Nuevo incidente reportado para tu área: {ticket.Titulo}",
                    $"Hola {destinatarioNombre},\n\n" +
                    $"Se ha reportado un nuevo ticket de soporte que corresponde a tu área:\n\n" +
                    $"  Ticket ID : {ticket.Id}\n" +
                    $"  Título    : {ticket.Titulo}\n" +
                    $"  Categoría : {categoriaNombre}\n" +
                    $"  Prioridad : {prioridad.Nombre}\n" +
                    $"  Descripción: {sanitizedDescription}\n\n" +
                    $"  Justificación IA: {aiResult.Justificacion}\n\n" +
                    $"Por favor revisa y toma este ticket a la brevedad en el panel.\n\n" +
                    $"— Sistema PIGI-PT");
            }

            catch (Exception ex)
            {
                // Si la IA o notificación falla, aún guardamos el ticket pero en estado PendienteDeAnalisis (estado base)
                Console.WriteLine($"Error procesando IA/Notificación: {ex.Message}");
            }

            // 6. Persistir mediante el repositorio
            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var catNombre = categoriaNombre ?? categorias.FirstOrDefault(c => c.Id == categoriaId)?.NombreCategoria;
            var opNombre = operadorAdecuado?.FullName;
            var creator = await _unitOfWork.Usuarios.GetByIdAsync(request.UserId);

            return TicketMapper.ToDto(ticket, catNombre, opNombre, creator?.FullName, creator?.Email);
        }
    }
}
