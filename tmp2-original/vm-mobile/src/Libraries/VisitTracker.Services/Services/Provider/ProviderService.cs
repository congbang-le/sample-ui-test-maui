namespace VisitTracker.Services;

public class ProviderService : BaseService<Provider>, IProviderService
{
    private readonly IProviderApi _api;
    private readonly IProviderStorage _storage;

    public ProviderService(IProviderApi api,
        IProviderStorage providerStorage) : base(providerStorage)
    {
        _api = api;
        _storage = providerStorage;
    }

    public async Task<Provider> LoginProvider(string code)
    {
        var provider = await _api.LoginProvider(code);
        if (provider == null)
            return null;

        await _storage.DeleteAll();
        return await _storage.InsertOrReplace(provider);
    }

    public async Task<Provider> GetLoggedInProvider()
    {
        return await _storage.GetLoggedInProvider();
    }
}