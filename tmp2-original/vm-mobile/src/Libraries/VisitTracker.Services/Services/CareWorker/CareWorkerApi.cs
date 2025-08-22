namespace VisitTracker.Services;

public class CareWorkerApi : ICareWorkerApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public CareWorkerApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<CareWorker> SyncCareWorker(int cwId)
    {
        return await _requestProvider.ExecuteAsync<CareWorker>(
            Constants.EndUrlGetProfileCareWorker, HttpMethod.Post, cwId
        );
    }
}