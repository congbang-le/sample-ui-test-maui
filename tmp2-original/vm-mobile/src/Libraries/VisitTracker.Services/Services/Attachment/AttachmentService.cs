namespace VisitTracker.Services;

public class AttachmentService : BaseService<Attachment>, IAttachmentService
{
    private readonly IAttachmentStorage _storage;
    private readonly IAttachmentApi _api;

    public AttachmentService(IAttachmentStorage attachmentStorage
        , IAttachmentApi api) : base(attachmentStorage)
    {
        _storage = attachmentStorage;
        _api = api;
    }

    public async Task<IList<Attachment>> GetAllByBaseVisitId(int id)
    {
        return await _storage.GetAllByBaseVisitId(id);
    }

    public async Task<IList<Attachment>> GetAllByVisitId(int id)
    {
        return await _storage.GetAllByVisitId(id);
    }

    public async Task<IList<Attachment>> GetAllByVisitTaskId(int id)
    {
        return await _storage.GetByTaskId(id);
    }

    public async Task<IList<Attachment>> GetAllByVisitMedicationId(int id)
    {
        return await _storage.GetByMedicationId(id);
    }

    public async Task<IList<Attachment>> GetAllByFluidId(int id)
    {
        return await _storage.GetByFluidId(id);
    }

    public async Task<IList<Attachment>> GetAllByBodyMapId(int id)
    {
        return await _storage.GetByBodyMapId(id);
    }

    public async Task<IList<Attachment>> GetAllByIncidentId(int id)
    {
        return await _storage.GetByIncidentId(id);
    }

    public async Task DeleteAllByVisitId(int visitId)
    {
        await _storage.DeleteAllByVisitId(visitId);
    }

    public async Task DeleteAllByBodyMapId(int bodyMapId)
    {
        await _storage.DeleteAllByBodyMapId(bodyMapId);
    }

    public async Task DeleteAllByIncidentId(int incidentId)
    {
        await _storage.DeleteAllByIncidentId(incidentId);
    }

    public async Task<IList<Attachment>> GetSignedAttachments(IList<int> attachmentIds)
    {
        var attachments = await _api.GetSignedAttachments(attachmentIds);
        if (attachments != null && attachments.Any())
            attachments.ToList().ForEach(i => i.FileName = Path.GetFileName(i.S3Url));

        return attachments;
    }

    public async Task<IList<Attachment>> GetSignedAttachments(IList<string> attachmentUrls)
    {
        var attachments = await _api.GetSignedAttachments(attachmentUrls);
        if (attachments != null && attachments.Any())
            attachments.ToList().ForEach(i => i.FileName = Path.GetFileName(i.S3Url));

        return attachments;
    }

    public async Task<IEnumerable<AttachmentUploadDto>> GetPresignedUrlsForUpload(IList<AttachmentUploadDto> urls)
    {
        return await _api.GetPresignedUrlsForUpload(urls);
    }
}