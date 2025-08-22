namespace VisitTracker.Services;

public interface IServiceUserAddressService : IBaseService<ServiceUserAddress>
{
    Task<ServiceUserAddress> GetActiveAddressNow(int serviceUserId);

    Task<ServiceUserAddress> GetActiveAddressByDate(int serviceUserId, DateTime date);
}