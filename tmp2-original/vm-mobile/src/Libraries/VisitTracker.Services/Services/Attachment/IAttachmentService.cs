namespace VisitTracker.Services;

public interface IAttachmentService : IBaseService<Attachment>
{
    Task<IList<Attachment>> GetAllByBaseVisitId(int id);

    Task<IList<Attachment>> GetAllByVisitId(int bookingId);

    Task<IList<Attachment>> GetAllByVisitTaskId(int taskId);

    Task<IList<Attachment>> GetAllByVisitMedicationId(int medicationId);

    Task<IList<Attachment>> GetAllByFluidId(int fluidId);

    Task<IList<Attachment>> GetAllByBodyMapId(int bodyMapId);

    Task<IList<Attachment>> GetAllByIncidentId(int incidentId);

    Task DeleteAllByVisitId(int bookingId);

    Task DeleteAllByBodyMapId(int bodyMapId);

    Task DeleteAllByIncidentId(int incidentId);

    Task<IList<Attachment>> GetSignedAttachments(IList<int> attachmentIds);

    Task<IList<Attachment>> GetSignedAttachments(IList<string> attachmentUrls);

    Task<IEnumerable<AttachmentUploadDto>> GetPresignedUrlsForUpload(IList<AttachmentUploadDto> urls);
}