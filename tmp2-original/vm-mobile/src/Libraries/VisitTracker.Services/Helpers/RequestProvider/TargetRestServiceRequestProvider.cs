using System.Net;
using System.Net.Http.Headers;

namespace VisitTracker.Services;

public class TargetRestServiceRequestProvider : IRestServiceRequestProvider
{
    public string UserAgent { get; set; } = "uag-JLBPnNgL6v9CkbeVZrfTYCXSgHHDvygg8X24Eg6qdzmKFhNbZwLh62Xh3J6t8Gfn";

    private Provider Provider { get; set; }
    private Profile Profile { get; set; }

    private readonly IProviderService _providerService;
    private readonly IProfileService _profileService;

    public TargetRestServiceRequestProvider(IProviderService providerService,
        IProfileService profileService)
    {
        _providerService = providerService;
        _profileService = profileService;
    }

    private async Task<string> GetProviderUrl(string url)
    {
        if (Provider == null)
            Provider = await _providerService.GetLoggedInProvider();

        if (Profile == null)
            Profile = await _profileService.GetLoggedInProfileUser();

        var serverUrl = Provider.ServerUrl;
#if DEBUG
        serverUrl = Constants.UrlServer;
#elif TEST
        serverUrl = Constants.UrlTestServer;
#endif

        if (!string.IsNullOrEmpty(serverUrl))
            return $"{serverUrl}/{url}";

        return "";
    }

    public async Task<T> ExecuteAsync<T>(string url, HttpMethod method, object data = null)
    {
        var absoluteUrl = await GetProviderUrl(url);
        var request = new HttpRequestMessage(method, absoluteUrl);
        request.Headers.UserAgent.ParseAdd(UserAgent);

        if (data != null)
            request.Content = new StringContent(JsonExtensions.Serialize(data), Constants.UrlEncoding, Constants.UrlContentType);

        if (Profile != null)
        {
            if (!string.IsNullOrEmpty(Profile.Token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Profile.Token);

            if (Profile.TokenValidTo == null || Profile.RefreshTokenValidTo == null || Convert.ToDateTime(Profile.RefreshTokenValidTo) < DateTimeExtensions.NowNoTimezone())
                throw new InvalidUserException();

            if (Profile.TokenValidTo != null && Convert.ToDateTime(Profile.TokenValidTo) < DateTimeExtensions.NowNoTimezone())
            {
                var isRefreshSuccess = await RefreshToken(Profile.Token);
                if (!isRefreshSuccess)
                    throw new InvalidUserException();
            }
        }

        HttpResponseMessage response;

        using (var client = new HttpClient() { DefaultRequestVersion = HttpVersion.Version20, Timeout = TimeSpan.FromSeconds(Constants.UrlResponseTimeOut) })
            response = await client.SendAsync(request).ConfigureAwait(false);

        if (url == Constants.EndUrlLogout)
        {
            Provider = null;
            Profile = null;
        }

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (typeof(T) == typeof(string))
                return (T)(object)json;
            if (string.IsNullOrEmpty(json))
                return default;
            return JsonExtensions.Deserialize<T>(json);
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized
                    || response.StatusCode == HttpStatusCode.Forbidden)
            throw new InvalidUserException();
        else if (response.StatusCode == HttpStatusCode.NotFound)
            throw new HttpRequestException();

        return default;
    }

    private async Task<bool> RefreshToken(string accessToken)
    {
        string url = await GetProviderUrl(Constants.EndUrlRefreshToken);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        var requestData = new
        {
            AccessToken = Profile.Token,
            RefreshToken = Profile.RefreshToken,
            DevicePlatform = DeviceInfo.Platform.ToString(),
        };

        request.Content = new StringContent(JsonExtensions.Serialize(requestData), Constants.UrlEncoding, Constants.UrlContentType);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd("uag-JLBPnNgL6v9CkbeVZrfTYCXSgHHDvygg8X24Eg6qdzmKFhNbZwLh62Xh3J6t8Gfn");

        HttpResponseMessage response;
        using (var client = new HttpClient() { DefaultRequestVersion = HttpVersion.Version20 })
            response = await client.SendAsync(request).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new Exception(response.ToString());

        var content = response.Content;
        if (content != null)
        {
            var json = await content.ReadAsStringAsync().ConfigureAwait(false);
            var refreshTokenResult = JsonExtensions.Deserialize<AuthResponse>(json);

            Profile.Token = refreshTokenResult.Token;
            Profile.TokenValidTo = refreshTokenResult.TokenValidTo;
            Profile.RefreshToken = refreshTokenResult.RefreshToken;
            Profile.RefreshTokenValidTo = refreshTokenResult.RefreshTokenValidTo;
            Profile = await _profileService.InsertOrReplace(Profile);

            return true;
        }

        return false;
    }
}