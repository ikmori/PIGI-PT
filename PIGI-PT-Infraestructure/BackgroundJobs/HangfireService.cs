using System;
using Hangfire;
using PIGI_PT_Application.Ports.Infrastructure;
using PIGI_PT_Infraestructure.BackgroundJobs.Tickets;

namespace PIGI_PT_Infraestructure.BackgroundJobs
{
    /// <summary>
    /// Adaptador que implementa IHangfireService para encolar tareas asíncronas en Hangfire.
    /// </summary>
    public class HangfireService : IHangfireService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public void EnqueueSanitization(Guid ticketId, string description)
        {
            _backgroundJobClient.Enqueue<TicketSanitizationJob>(job => job.ExecuteAsync(ticketId, description));
        }

        public void EnqueueClassification(Guid ticketId, string description)
        {
            _backgroundJobClient.Enqueue<TicketClassificationJob>(job => job.ExecuteAsync(ticketId, description));
        }
    }
}
