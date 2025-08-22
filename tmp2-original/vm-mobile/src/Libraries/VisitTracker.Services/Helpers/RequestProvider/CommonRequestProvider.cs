using System.Net;

namespace VisitTracker.Services;

public class CommonRequestProvider : IRestServiceRequestProvider
{
    public string UserAgent { get; set; } = "uag-4bt9UEj58tsETpsLA8YSd32hjT4kCQ68HwLZByZ3y6dCq76rc8pcj3mnBeJRmRQd";

    private string GetProviderUrl(string url)
    {
        var _providerUrl = Constants.ProviderLoginServer;
        if (!string.IsNullOrEmpty(_providerUrl))
            return $"{_providerUrl}/{url}";

        return "";
    }

    public async Task<T> ExecuteAsync<T>(string url, HttpMethod method, object data = null)
    {
        var absoluteUrl = GetProviderUrl(url);
        var request = new HttpRequestMessage(method, absoluteUrl);
        request.Headers.UserAgent.ParseAdd(UserAgent);
        request.Headers.Add("X-PSK", "QxKa5kgjpDeb4ahX6HnBPzBGID6MXZ4FzmlqmNBzNK5ePa3hsO");

        if (data != null)
            request.Content = new StringContent(JsonExtensions.Serialize(data), Constants.UrlEncoding, Constants.UrlContentType);

        if (url != Constants.EndUrlProviderLogin)
            throw new Exception("Access Denied!");

        HttpResponseMessage response;

        using (var client = new HttpClient() { DefaultRequestVersion = HttpVersion.Version20, Timeout = TimeSpan.FromSeconds(Constants.UrlResponseTimeOut) })
            response = await client.SendAsync(request).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonExtensions.Deserialize<T>(json);
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new InvalidUserException();
        else throw new Exception(response?.ToString());
    }
}