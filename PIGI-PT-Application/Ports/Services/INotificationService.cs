namespace PIGI_PT_Application.Ports.Services
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendPushNotificationAsync(Guid userId, string message);
    }
}
