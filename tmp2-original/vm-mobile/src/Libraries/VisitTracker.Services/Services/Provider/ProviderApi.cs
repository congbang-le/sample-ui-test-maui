namespace VisitTracker.Services;

public class ProviderApi : IProviderApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public ProviderApi(CommonRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<Provider> LoginProvider(string providerCode)
    {
        return await _requestProvider.ExecuteAsync<Provider>(
            Constants.EndUrlProviderLogin, HttpMethod.Post,
            new { LoginCode = providerCode }
        );
    }
}