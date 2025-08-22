namespace VisitTracker.Services;

public class SyncApi : ISyncApi
{
    private readonly IRestServiceRequestProvider _requestProvider;
    private readonly IProfileService _profileService;

    public SyncApi(TargetRestServiceRequestProvider requestProvider,
        IProfileService profileService)
    {
        _requestProvider = requestProvider;
        _profileService = profileService;
    }

    public async Task<SyncResponse> SyncDataToServer(IEnumerable<Sync> data)
    {
        var currentProfile = await _profileService.GetLoggedInProfileUser();
        var syncUrl = currentProfile?.Type switch
        {
            nameof(EUserType.CAREWORKER) => Constants.EndUrlSyncData,
            nameof(EUserType.NEXTOFKIN) or nameof(EUserType.SERVICEUSER) => Constants.EndUrlSuSyncData,
            nameof(EUserType.SUPERVISOR) => Constants.EndUrlSupSyncData,
            _ => throw new NotImplementedException()
        };

        return await _requestProvider.ExecuteAsync<SyncResponse>(
            syncUrl, HttpMethod.Post,
            JsonExtensions.Serialize(data)
        );
    }
}