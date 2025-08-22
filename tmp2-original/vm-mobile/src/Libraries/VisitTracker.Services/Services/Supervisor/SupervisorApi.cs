namespace VisitTracker.Services;

public class SupervisorApi : ISupervisorApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public SupervisorApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<SupervisorDataResponse> DownloadAll()
    {
        return await _requestProvider.ExecuteAsync<SupervisorDataResponse>(
                Constants.EndUrlSupDownloadAll, HttpMethod.Get
            );
    }

    public async Task<SupervisorFormsResponse> GetFormDetailsBySup(int supId)
    {
        return await _requestProvider.ExecuteAsync<SupervisorFormsResponse>(
               Constants.EndUrlSupFormsCount, HttpMethod.Post, supId
           );
    }
}