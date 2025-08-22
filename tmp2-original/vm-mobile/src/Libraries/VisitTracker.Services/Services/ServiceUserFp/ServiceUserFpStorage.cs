namespace VisitTracker.Services;

public class ServiceUserFpStorage : BaseStorage<ServiceUserFp>, IServiceUserFpStorage
{
    public ServiceUserFpStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<ServiceUserFp> GetByServiceUserId(int serviceUserId)
    {
        return await FirstOrDefault(q => q.Where(x => x.ServiceUserId == serviceUserId));
    }
}