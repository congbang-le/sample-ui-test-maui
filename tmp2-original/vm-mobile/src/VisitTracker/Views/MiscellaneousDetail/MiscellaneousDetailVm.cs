using System.Text.Json;
using System.Web;

namespace VisitTracker;

[QueryProperty(nameof(Title), nameof(Title))]
[QueryProperty(nameof(ServerUrl), nameof(ServerUrl))]
public class MiscellaneousDetailVm : BaseVm, IQueryAttributable
{
    public string ServerUrl { get; set; }
    public Dictionary<string, string> Params { get; set; }
    public UrlWebViewSource Source { get; set; }
    public CookieContainer Cookies { get; set; }

    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    public MiscellaneousDetailVm()
    {
        ExitCommand = ReactiveCommand.CreateFromTask(Exit);
        BindBusyWithException(ExitCommand);
    }

    protected override async Task Init()
    {
        if (AppData.Current.CurrentProfile == null || AppData.Current.Provider == null || string.IsNullOrEmpty(ServerUrl))
            return;

        Cookies = SystemHelper.Current.OpenTpUrl(ServerUrl);
        var url = $"{ServerUrl}";
        // Append query parameters from Params dictionary
        if (Params != null && Params.Any())
        {
            var queryString = string.Join("&", Params.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            url += (url.Contains("?") ? "&" : "?") + queryString;
        }
        Source = new UrlWebViewSource { Url = url };

        await Task.CompletedTask;
    }

    public async Task Exit()
    {
        await Task.Run(() =>
        {
#if ANDROID
            Android.Webkit.WebStorage.Instance.DeleteAllData();

            Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
            Android.Webkit.CookieManager.Instance.Flush();
#endif
        });
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("ServerUrl", out var serverUrl))
            ServerUrl = serverUrl as string;
        if (query.TryGetValue("AdditionalParams", out var additionalParamsJson))
        {
            string paramJsonStr = additionalParamsJson as string;
            if (!string.IsNullOrEmpty(paramJsonStr))
            {
                try
                {
                    // Use additionalParams as key-value pairs
                    string decodedJson = HttpUtility.UrlDecode(paramJsonStr); // Decode URL-encoded JSON
                    Params = JsonSerializer.Deserialize<Dictionary<string, string>>(decodedJson);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine("Error parsing params JSON: " + ex.Message);
                }
            }
        }

        RefreshOnAppear = true;
        await InitCommand.Execute();
    }
}