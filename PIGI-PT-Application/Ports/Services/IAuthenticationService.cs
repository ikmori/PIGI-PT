namespace PIGI_PT_Application.Ports.Services
{
    public interface IAuthenticationService
    {
        Task<string> GenerateTokenAsync(Guid userId, string userName, string rol);
        Task<bool> ValidateTokenAsync(string token);
    }
}
