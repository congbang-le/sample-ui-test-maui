namespace VisitTracker.Services;

public interface ICareWorkerApi
{
    Task<CareWorker> SyncCareWorker(int cwId);
}