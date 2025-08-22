namespace VisitTracker.Services;

public class ServiceUserAddressService : BaseService<ServiceUserAddress>, IServiceUserAddressService
{
    private readonly IServiceUserAddressStorage _storage;

    public ServiceUserAddressService(IServiceUserAddressStorage serviceUserAddressStorage)
        : base(serviceUserAddressStorage)
    {
        _storage = serviceUserAddressStorage;
    }

    public async Task<ServiceUserAddress> GetActiveAddressNow(int serviceUserId)
    {
        return await _storage.GetActiveAddressNow(serviceUserId);
    }

    public async Task<ServiceUserAddress> GetActiveAddressByDate(int serviceUserId, DateTime date)
    {
        return await _storage.GetActiveAddressByDate(serviceUserId, date);
    }
}