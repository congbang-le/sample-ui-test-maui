namespace VisitTracker.Services;

public interface IProviderStorage : IBaseStorage<Provider>
{
    Task<Provider> GetLoggedInProvider();
}