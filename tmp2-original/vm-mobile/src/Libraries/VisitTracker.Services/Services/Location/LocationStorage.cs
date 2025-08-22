namespace VisitTracker.Services;

public class LocationStorage : BaseStorage<VisitLocation>, ILocationStorage
{
    private readonly IBookingDetailStorage _bookingDetailStorage;

    public LocationStorage(ISecuredKeyProvider keyProvider,
        IBookingDetailStorage bookingDetailStorage) : base(keyProvider)
    {
        _bookingDetailStorage = bookingDetailStorage;
    }

    public async Task<IList<VisitLocation>> GetAllByServiceUserFp(int id)
    {
        return await Select(q => q.Where(x => x.ServiceUserFpId == id));
    }

    public async Task<IList<VisitLocation>> GetAllUnSyncByBookingDetail(int id)
    {
        return await Select(q => q.Where(x => x.VisitId == id && x.IsSync == false));
    }

    public async Task<bool> DeleteAllByVisitEmpty()
    {
        var allLocations = await GetAll();
        var ids = allLocations
            .Where(i => i.VisitId == 0 || i.VisitId == null)
            .Select(x => x.Id);

        await DeleteAllByIds(ids);
        return true;
    }

    public async Task<bool> DeleteAllByServiceUserFpEmpty()
    {
        var allLocations = await GetAll();
        var ids = allLocations
            .Where(i => i.ServiceUserFpId == 0 || i.ServiceUserFpId == null)
            .Select(x => x.Id);

        await DeleteAllByIds(ids);
        return true;
    }

    public async Task<IList<VisitLocation>> GetAllByVisitId(int id)
    {
        var allLocations = await GetAll();
        var result = allLocations
            .Where(args => args.VisitId == id)
            .ToList();

        return result ?? new List<VisitLocation>();
    }

    public async Task DeleteAllByVisitId(int visitId)
    {
        var locations = await Select(q => q.Where(x => x.VisitId == visitId));
        if (locations != null && locations.Any())
            await DeleteAllByIds(locations.Select(x => x.Id));
    }

    public async Task<IList<VisitLocation>> GetAllUnSyncSupLocation(int supVisitId)
    {
        return await Select(q => q.Where(x => x.SupervisorVisitId == supVisitId && x.IsSync == false));
    }

    public async Task<bool> DeleteAllBySup(int supVisitId)
    {
        var allSupervisorLocations = await GetAll();
        var ids = allSupervisorLocations
            .Where(i => i.SupervisorVisitId == supVisitId)
            .Select(x => x.Id);

        await DeleteAllByIds(ids);
        return true;
    }

    public async Task<IList<VisitLocation>> GetBySupVisit(int id)
    {
        var allSupervisorLocations = await GetAll();
        var result = allSupervisorLocations
            .Where(args => args.SupervisorVisitId == id)
            .ToList();

        return result ?? new List<VisitLocation>();
    }
}