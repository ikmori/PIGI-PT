namespace PIGI_PT_Application.Ports.Services
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
