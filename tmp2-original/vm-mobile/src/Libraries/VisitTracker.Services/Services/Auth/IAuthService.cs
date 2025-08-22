namespace VisitTracker.Services;

public interface IAuthService
{
    Task<Profile> Login(string email, string password);

    Task<bool> Logout();

    Task<DateTime?> GetServerTime();

    Task<bool> RegisterDeviceForPushNotifications();

    Task<bool> CheckServerAvailability();

    Task<TpAuthResponse> GetTpTokenByUser(int userId);
}