namespace VisitTracker.Services;

public interface ICareWorkerService : IBaseService<CareWorker>
{
    Task SyncCareWorker(int cwId);

    Task<IList<CareWorker>> DownloadProfilePictures(List<CareWorker> users);
}