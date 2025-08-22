namespace VisitTracker.Services;

public interface IServiceUserAddressStorage : IBaseStorage<ServiceUserAddress>
{
    Task<ServiceUserAddress> GetActiveAddressNow(int serviceUserId);

    Task<ServiceUserAddress> GetActiveAddressByDate(int serviceUserId, DateTime date);
}