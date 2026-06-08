namespace PIGI_PT_Application.Ports.Infrastructure
{
    public interface IHangfireService
    {
        void EnqueueSanitization(Guid ticketId, string description);
        void EnqueueClassification(Guid ticketId, string description);
    }
}
