namespace VisitTracker.Services;

public class CareWorkerStorage : BaseStorage<CareWorker>, ICareWorkerStorage
{
    public CareWorkerStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<CareWorker> GetLoggedInCareWorker()
    {
        var allCareWorkers = await GetAll();
        return allCareWorkers.FirstOrDefault();
    }
}