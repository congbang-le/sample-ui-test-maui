namespace VisitTracker.Services;

public interface IServiceUserFpStorage : IBaseStorage<ServiceUserFp>
{
    Task<ServiceUserFp> GetByServiceUserId(int serviceUserId);
}