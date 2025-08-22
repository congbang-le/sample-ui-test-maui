namespace VisitTracker.Services;

public class ServiceUserStorage : BaseStorage<ServiceUser>, IServiceUserStorage
{
    private readonly ILocationCentroidStorage _locationCentroidStorage;

    public ServiceUserStorage(ISecuredKeyProvider keyProvider
        , ILocationCentroidStorage locationCentroidStorage) : base(keyProvider)
    {
        _locationCentroidStorage = locationCentroidStorage;
    }

    public async Task<IList<ServiceUser>> GetAllByMissingFingerprints(DevicePlatform devicePlatform)
    {
        if (devicePlatform == DevicePlatform.Android)
            return await Select(q => q.Where(x => x.AndroidFpAvailable == null || x.AndroidFpAvailable == false));

        if (devicePlatform == DevicePlatform.iOS)
            return await Select(q => q.Where(x => x.iOSFpAvailable == null || x.iOSFpAvailable == false));

        return null;
    }

    public async Task<IList<ServiceUser>> GetAllByMissingGroundTruths()
    {
        var serviceUsers = await GetAll();
        var centroids = await _locationCentroidStorage.GetAllByIds(serviceUsers.Select(x => x.Id), "ServiceUserId");
        var centroidsG = centroids.Where(x => x.LocationClass == ELocationClass.G.ToString());
        return serviceUsers.Where(x => !centroidsG.Any(y => y.ServiceUserId == x.Id))?.ToList();
    }
}