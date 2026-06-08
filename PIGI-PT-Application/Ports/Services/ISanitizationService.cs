namespace PIGI_PT_Application.Ports.Services
{
    public interface ISanitizationService
    {
        Task SanitizeTicketAsync(Guid ticketId, string originalDescription);
    }
}
