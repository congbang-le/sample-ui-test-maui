namespace VisitTracker.Services;

public class ServiceUserService : BaseService<ServiceUser>, IServiceUserService
{
    private readonly IServiceUserStorage _storage;
    private readonly IServiceUserApi _api;
    private readonly ILocationCentroidService _locationCentroidService;
    private readonly IServiceUserAddressService _serviceUserAddressService;
    private readonly IFileSystemService _fileSystemService;

    public ServiceUserService(IServiceUserStorage serviceUserStorage,
        IServiceUserApi api,
        ILocationCentroidService locationCentroidService,
        IServiceUserAddressService serviceUserAddressService,
        IFileSystemService fileSystemService) : base(serviceUserStorage)
    {
        _storage = serviceUserStorage;
        _api = api;
        _locationCentroidService = locationCentroidService;
        _serviceUserAddressService = serviceUserAddressService;
        _fileSystemService = fileSystemService;
    }

    public async Task SyncServiceUser(int suId)
    {
        var response = await _api.SyncServiceUser(suId);
        if (response != null && response.ServiceUser != null)
        {
            await DownloadProfilePictures(new List<ServiceUser> { response.ServiceUser });
            response.ServiceUser = await _storage.InsertOrReplace(response.ServiceUser);

            if (response.Centroids != null && response.Centroids.Any())
            {
                await _locationCentroidService.DeleteAllByIds([response.ServiceUser.Id], "ServiceUserId");
                await _locationCentroidService.InsertOrReplace(response.Centroids);
            }

            if (response.ServiceUserAddresses != null && response.ServiceUserAddresses.Any())
            {
                await _serviceUserAddressService.DeleteAllByIds([response.ServiceUser.Id], "ServiceUserId");
                await _serviceUserAddressService.InsertOrReplace(response.ServiceUserAddresses);
            }
        }
    }

    public async Task<IList<ServiceUser>> GetAllByMissingFingerprints(DevicePlatform devicePlatform)
    {
        return await _storage.GetAllByMissingFingerprints(devicePlatform);
    }

    public async Task<IList<ServiceUser>> GetAllByMissingGroundTruths()
    {
        return await _storage.GetAllByMissingGroundTruths();
    }

    public async Task<IList<ServiceUser>> DownloadProfilePictures(List<ServiceUser> users)
    {
        foreach (var serviceUser in users.Where(y => !string.IsNullOrEmpty(y.ImageUrl)))
            serviceUser.ImageUrl = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(serviceUser.ImageUrl));
        if (users.Any(x => !string.IsNullOrEmpty(x.SignedImageUrl)))
        {
            var attachmentList = users.Where(y => !string.IsNullOrEmpty(y.ImageUrl) && !File.Exists(y.ImageUrl))
                .Select(x => new Attachment
                {
                    S3SignedUrl = x.SignedImageUrl,
                    S3Url = x.ImageUrl
                }).ToList();
            if (attachmentList.Any())
                await _fileSystemService.DownloadFiles(attachmentList);
        }
        users.ForEach(x => x.SignedImageUrl = null);
        return users;
    }
}