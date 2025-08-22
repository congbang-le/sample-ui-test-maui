namespace VisitTracker.Services;

public class IncidentService : BaseService<Incident>, IIncidentService
{
    private readonly IIncidentStorage _storage;
    private readonly IFileSystemService _fileSystemService;
    private readonly IBookingStorage _bookingStorage;
    private readonly ICareWorkerStorage _careWorkerStorage;
    private readonly IBodyMapStorage _bodyMapStorage;
    private readonly IAttachmentStorage _attachmentStorage;
    private readonly IIncidentApi _api;

    public IncidentService(IIncidentStorage incidentStorage,
        IFileSystemService fileSystemService,
        IBookingStorage bookingStorage,
        ICareWorkerStorage careWorkerStorage,
        IBodyMapStorage bodyMapStorage,
        IAttachmentStorage attachmentStorage,
        IIncidentApi api) : base(incidentStorage)
    {
        _storage = incidentStorage;
        _fileSystemService = fileSystemService;
        _bookingStorage = bookingStorage;
        _careWorkerStorage = careWorkerStorage;
        _bodyMapStorage = bodyMapStorage;
        _attachmentStorage = attachmentStorage;
        _api = api;
    }

    public async Task<IList<Incident>> GetAllAdhoc()
    {
        return await _storage.GetAllAdhoc();
    }

    public async Task DeleteAdhocIncidents()
    {
        var incidents = await GetAllAdhoc();
        if (incidents != null && incidents.Any())
        {
            var incidentIds = incidents.Select(i => i.Id).ToList();

            var bodyMaps = await _bodyMapStorage.GetAllByIds(incidentIds, "IncidentId");
            var bodyMapIds = bodyMaps.Select(i => i.Id).ToList();
            await _bodyMapStorage.DeleteAllByIds(bodyMapIds);

            var bodyMapAttachments = await _attachmentStorage.GetAllByIds(bodyMapIds, "BodyMapId");
            if (bodyMapAttachments != null && bodyMapAttachments.Any())
            {
                await _fileSystemService.DeleteFiles(bodyMapAttachments);
                await _attachmentStorage.DeleteAllByIds(bodyMapAttachments.Select(x => x.Id));
            }

            var attachmentsToDelete = await _attachmentStorage.GetAllByIds(incidentIds, "IncidentId");
            if (attachmentsToDelete != null && attachmentsToDelete.Any())
            {
                await _fileSystemService.DeleteFiles(attachmentsToDelete);
                await _attachmentStorage.DeleteAllByIds(attachmentsToDelete.Select(x => x.Id));
            }
        }
    }

    public async Task<IList<Incident>> GetAllByVisitId(int id)
    {
        return await _storage.GetAllByVisitId(id);
    }

    public async Task<IList<IncidentResponseDto>> GetAllByServiceUser(int suId)
    {
        return await _api.GetAllByServiceUser(suId);
    }

    public async Task<IList<IncidentResponseDto>> GetAllByCareWorker(int cwId)
    {
        return await _api.GetAllByCareWorker(cwId);
    }

    public async Task<IncidentDetailResponse> GetIncidentDetail(int incidentId)
    {
        return await _api.GetIncidentDetail(incidentId);
    }

    public async Task<bool> UploadIncidentReportAdhoc(IncidentAdhocRequest incident)
    {
        return await _api.UploadIncidentReportAdhoc(incident);
    }
}