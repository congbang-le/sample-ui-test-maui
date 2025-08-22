namespace VisitTracker.Services;

public class ServiceUserFpService : BaseService<ServiceUserFp>, IServiceUserFpService
{
    private readonly IServiceUserFpStorage _storage;

    public ServiceUserFpService(IServiceUserFpStorage storage) : base(storage)
    {
        _storage = storage;
    }
}