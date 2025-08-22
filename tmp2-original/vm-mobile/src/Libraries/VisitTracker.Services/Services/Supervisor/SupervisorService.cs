namespace VisitTracker.Services;

public class SupervisorService : ISupervisorService
{
    private readonly ISupervisorApi _api;
    private readonly IFileSystemService _fileSystemService;
    private readonly ICodeStorage _codeStorage;
    private readonly IVisitMessageStorage _visitMessageStorage;
    private readonly ICareWorkerStorage _careWorkerStorage;
    private readonly ICareWorkerService _careWorkerService;
    private readonly IServiceUserAddressStorage _serviceUserAddressStorage;
    private readonly IServiceUserFpStorage _serviceUserFpStorage;
    private readonly ILocationCentroidStorage _locationCentroidStorage;
    private readonly IServiceUserStorage _serviceUserStorage;
    private readonly IServiceUserService _serviceUserService;

    public SupervisorService(ISupervisorApi SupervisorApi,
        IFileSystemService fileSystemService,
        ICodeStorage codeStorage,
        IVisitMessageStorage visitMessageStorage,
        ICareWorkerStorage careWorkerStorage,
        ICareWorkerService careWorkerService,
        IServiceUserAddressStorage serviceUserAddressStorage,
        IServiceUserFpStorage serviceUserFpStorage,
        ILocationCentroidStorage locationCentroidStorage,
        IServiceUserStorage serviceUserStorage,
        IServiceUserService serviceUserService)
    {
        _api = SupervisorApi;
        _fileSystemService = fileSystemService;
        _codeStorage = codeStorage;
        _visitMessageStorage = visitMessageStorage;
        _careWorkerStorage = careWorkerStorage;
        _careWorkerService = careWorkerService;
        _serviceUserAddressStorage = serviceUserAddressStorage;
        _serviceUserFpStorage = serviceUserFpStorage;
        _locationCentroidStorage = locationCentroidStorage;
        _serviceUserStorage = serviceUserStorage;
        _serviceUserService = serviceUserService;
    }

    public async Task<bool> SyncData()
    {
        var response = await _api.DownloadAll();
        if (response == null)
            return false;

        await _codeStorage.DeleteAll();
        if (response.Codes != null)
            await _codeStorage.InsertOrReplace(response.Codes);

        await _visitMessageStorage.DeleteAll();
        if (response.VisitMessages != null)
            await _visitMessageStorage.InsertOrReplace(response.VisitMessages);

        await _careWorkerStorage.DeleteAll();
        if (response.CareWorkers != null && response.CareWorkers.Any())
        {
            await _careWorkerService.DownloadProfilePictures(response.CareWorkers);
            await _careWorkerStorage.InsertOrReplace(response.CareWorkers);
        }

        await _serviceUserStorage.DeleteAll();
        if (response.ServiceUsers != null && response.ServiceUsers.Any())
        {
            await _serviceUserService.DownloadProfilePictures(response.ServiceUsers);
            await _serviceUserStorage.InsertOrReplace(response.ServiceUsers);
        }

        await _serviceUserAddressStorage.DeleteAll();
        if (response.ServiceUsersAddresses != null)
            await _serviceUserAddressStorage.InsertOrReplace(response.ServiceUsersAddresses);

        await _locationCentroidStorage.DeleteAll();
        if (response.Centroids != null)
            await _locationCentroidStorage.InsertOrReplace(response.Centroids);

        await _serviceUserFpStorage.DeleteAll();
        if (response.ServiceUserFps != null)
            await _serviceUserFpStorage.InsertOrReplace(response.ServiceUserFps);

        return true;
    }

    public async Task<SupervisorFormsResponse> GetFormDetailsBySup(int supId)
    {
        return await _api.GetFormDetailsBySup(supId);
    }
}