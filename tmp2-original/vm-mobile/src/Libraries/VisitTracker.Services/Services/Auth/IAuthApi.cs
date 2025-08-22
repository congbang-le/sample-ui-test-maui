namespace VisitTracker.Services;

public interface IAuthApi
{
    Task<Profile> GetTokenAndCWDetail(string userName, string password);

    Task<DateTime> GetServerTime();

    Task<bool> RegisterDeviceForPushNotifications();

    Task<bool> CheckServerAvailability();

    Task<TpAuthResponse> GetTpTokenByUser();

    Task<bool> Logout();
}