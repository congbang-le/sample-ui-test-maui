namespace VisitTracker.Services;

public class AuthApi : IAuthApi
{
    private readonly IAppPreference _appPreference;
    private readonly IRestServiceRequestProvider _requestProvider;

    public AuthApi(TargetRestServiceRequestProvider requestProvider,
        IAppPreference appPreference)
    {
        _requestProvider = requestProvider;
        _appPreference = appPreference;
    }

    public async Task<Profile> GetTokenAndCWDetail(string userName, string password)
    {
        return await _requestProvider.ExecuteAsync<Profile>(
            Constants.EndUrlLogin, HttpMethod.Post,
            new
            {
                Email = userName,
                Password = password,
                DeviceInfo = _appPreference.DeviceInfo,
                PushToken = _appPreference.PushToken,
                DeviceId = _appPreference.DeviceID
            }
        );
    }

    public async Task<DateTime> GetServerTime()
    {
        return await _requestProvider.ExecuteAsync<DateTime>(
            Constants.EndUrlServerTime, HttpMethod.Get
        );
    }

    public async Task<bool> RegisterDeviceForPushNotifications()
    {
        if (string.IsNullOrEmpty(_appPreference.PushToken))
            return false;

        return await _requestProvider.ExecuteAsync<bool>(
            Constants.EndUrlRegisterDevice, HttpMethod.Post,
            new
            {
                DeviceInfo = _appPreference.DeviceInfo,
                PushToken = _appPreference.PushToken,
                DeviceId = _appPreference.DeviceID
            }
        );
    }

    public async Task<bool> CheckServerAvailability()
    {
        return await _requestProvider.ExecuteAsync<bool>(
                   Constants.EndUrlServerAvailability, HttpMethod.Get
               );
    }

    public async Task<TpAuthResponse> GetTpTokenByUser()
    {
        return await _requestProvider.ExecuteAsync<TpAuthResponse>(Constants.EndUrlTpToken, HttpMethod.Get);
    }

    public async Task<bool> Logout()
    {
        return await _requestProvider.ExecuteAsync<bool>(Constants.EndUrlLogout, HttpMethod.Post);
    }
}