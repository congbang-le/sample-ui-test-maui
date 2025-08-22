namespace VisitTracker.Services;

public interface ICareWorkerStorage : IBaseStorage<CareWorker>
{
    Task<CareWorker> GetLoggedInCareWorker();
}