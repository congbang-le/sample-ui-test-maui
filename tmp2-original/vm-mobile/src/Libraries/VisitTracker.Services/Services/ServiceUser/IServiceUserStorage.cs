namespace VisitTracker.Services;

public interface IServiceUserStorage : IBaseStorage<ServiceUser>
{
    Task<IList<ServiceUser>> GetAllByMissingFingerprints(DevicePlatform devicePlatform);

    Task<IList<ServiceUser>> GetAllByMissingGroundTruths();
}