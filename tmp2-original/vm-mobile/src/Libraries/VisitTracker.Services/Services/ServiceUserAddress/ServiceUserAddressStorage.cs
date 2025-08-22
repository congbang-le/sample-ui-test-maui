namespace VisitTracker.Services;

public class ServiceUserAddressStorage : BaseStorage<ServiceUserAddress>, IServiceUserAddressStorage
{
    public ServiceUserAddressStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<ServiceUserAddress> GetActiveAddressNow(int serviceUserId)
    {
        var date = DateTimeExtensions.NowNoTimezone();
        return await FirstOrDefault(q => q.Where(x => x.ServiceUserId == serviceUserId
            && x.EffectiveFromTicks <= date.Ticks && (x.EffectiveToTicks == default || x.EffectiveToTicks >= date.Ticks)));
    }

    public async Task<ServiceUserAddress> GetActiveAddressByDate(int serviceUserId, DateTime date)
    {
        return await FirstOrDefault(q => q.Where(x => x.ServiceUserId == serviceUserId
            && x.EffectiveFromTicks <= date.Ticks && (x.EffectiveToTicks == default || x.EffectiveToTicks >= date.Ticks)));
    }
}