namespace VisitTracker.Services;

public interface IServiceUserService : IBaseService<ServiceUser>
{
    Task SyncServiceUser(int suId);

    Task<IList<ServiceUser>> GetAllByMissingFingerprints(DevicePlatform devicePlatform);

    Task<IList<ServiceUser>> GetAllByMissingGroundTruths();

    Task<IList<ServiceUser>> DownloadProfilePictures(List<ServiceUser> users);
}