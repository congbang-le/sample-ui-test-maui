namespace VisitTracker.Services;

public class LocationCentroidService : BaseService<LocationCentroid>, ILocationCentroidService
{
    private readonly ILocationCentroidStorage _storage;
    private readonly ILocationCentroidApi _api;

    public LocationCentroidService(ILocationCentroidStorage locationCentroidStorage,
        ILocationCentroidApi api) : base(locationCentroidStorage)
    {
        _storage = locationCentroidStorage;
        _api = api;
    }

    public async Task<LocationCentroid> GetGroundTruth(int serviceUserId)
    {
        return await _storage.GetGroundTruth(serviceUserId);
    }

    public async Task<LocationCentroid> UpdateGroundTruth(LocationCentroid LocationCentroid)
    {
        return await _api.UpdateGroundTruth(LocationCentroid);
    }
}