namespace VisitTracker.Services;

public class LocationService : BaseService<VisitLocation>, ILocationService
{
    private readonly ILocationStorage _storage;

    public LocationService(ILocationStorage locationStorage) : base(locationStorage)
    {
        _storage = locationStorage;
    }

    public async Task<IList<VisitLocation>> GetAllByServiceUserFp(int serviceUserId)
    {
        return await _storage.GetAllByServiceUserFp(serviceUserId);
    }

    public async Task<IList<VisitLocation>> GetAllUnSyncLocation(int bookingId)
    {
        return await _storage.GetAllUnSyncByBookingDetail(bookingId);
    }

    public async Task<IList<VisitLocation>> GetAllByVisitId(int id)
    {
        return await _storage.GetAllByVisitId(id);
    }

    public async Task<bool> DeleteAllByVisitEmpty()
    {
        return await _storage.DeleteAllByVisitEmpty();
    }

    public async Task<bool> DeleteAllByServiceUserFpEmpty()
    {
        return await _storage.DeleteAllByServiceUserFpEmpty();
    }

    public async Task DeleteAllByVisitId(int id)
    {
        await _storage.DeleteAllByVisitId(id);
    }

    public async Task<IList<VisitLocation>> GetAllUnSyncSupervisorLocation(int supVisitId)
    {
        return await _storage.GetAllUnSyncSupLocation(supVisitId);
    }

    public async Task<bool> DeleteAllBySup(int supVisitId)
    {
        return await _storage.DeleteAllBySup(supVisitId);
    }

    public async Task<IList<VisitLocation>> GetBySupVisit(int id)
    {
        return await _storage.GetBySupVisit(id);
    }
}