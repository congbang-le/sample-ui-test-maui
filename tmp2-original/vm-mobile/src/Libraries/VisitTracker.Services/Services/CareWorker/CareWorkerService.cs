namespace VisitTracker.Services;

public class CareWorkerService : BaseService<CareWorker>, ICareWorkerService
{
    private readonly ICareWorkerStorage _storage;
    private readonly ICareWorkerApi _api;
    private readonly IFileSystemService _fileSystemService;

    public CareWorkerService(ICareWorkerStorage careWorkerStorage,
        ICareWorkerApi api,
        IFileSystemService fileSystemService) : base(careWorkerStorage)
    {
        _storage = careWorkerStorage;
        _api = api;
        _fileSystemService = fileSystemService;
    }

    public async Task SyncCareWorker(int cwId)
    {
        var CareWorker = await _api.SyncCareWorker(cwId);
        if (CareWorker != null)
        {
            await DownloadProfilePictures(new List<CareWorker> { CareWorker });
            await _storage.InsertOrReplace(CareWorker);
        }
    }

    public async Task<IList<CareWorker>> DownloadProfilePictures(List<CareWorker> users)
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