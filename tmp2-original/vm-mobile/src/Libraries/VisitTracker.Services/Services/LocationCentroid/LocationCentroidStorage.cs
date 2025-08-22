namespace VisitTracker.Services;

public class LocationCentroidStorage : BaseStorage<LocationCentroid>, ILocationCentroidStorage
{
    public LocationCentroidStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<LocationCentroid> GetGroundTruth(int id)
    {
        var groundTruth = ELocationClass.G.ToString();
        return await FirstOrDefault(q => q.Where(x => x.ServiceUserId == id && x.LocationClass == groundTruth));
    }
}