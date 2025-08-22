namespace VisitTracker.Services;

public class ProviderStorage : BaseStorage<Provider>, IProviderStorage
{
    public ProviderStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<Provider> GetLoggedInProvider()
    {
        var allProviders = await GetAll();
        return allProviders?.FirstOrDefault();
    }
}