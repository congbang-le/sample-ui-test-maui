namespace VisitTracker.Services;

public interface IProviderApi
{
    Task<Provider> LoginProvider(string providerCode);
}