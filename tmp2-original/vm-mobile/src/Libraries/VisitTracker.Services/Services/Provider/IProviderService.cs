namespace VisitTracker.Services;

public interface IProviderService : IBaseService<Provider>
{
    Task<Provider> LoginProvider(string code);

    Task<Provider> GetLoggedInProvider();
}