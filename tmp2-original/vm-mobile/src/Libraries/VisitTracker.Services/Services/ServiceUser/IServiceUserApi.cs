namespace VisitTracker.Services;

public interface IServiceUserApi
{
    Task<ServiceUserResponse> SyncServiceUser(int suId);
}