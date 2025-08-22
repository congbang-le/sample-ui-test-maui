namespace VisitTracker.Services;

public class ServiceUserApi : IServiceUserApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public ServiceUserApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<ServiceUserResponse> SyncServiceUser(int suId)
    {
        return await _requestProvider.ExecuteAsync<ServiceUserResponse>(
            Constants.EndUrlGetProfileServiceUser, HttpMethod.Post, suId
        );
    }
}