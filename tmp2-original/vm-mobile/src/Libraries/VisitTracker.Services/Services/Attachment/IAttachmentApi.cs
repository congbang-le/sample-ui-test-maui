namespace VisitTracker.Services;

public interface IAttachmentApi
{
    Task<IList<Attachment>> GetSignedAttachments(IList<int> attachmentIds);

    Task<IList<Attachment>> GetSignedAttachments(IList<string> attachmentUrls);

    Task<IEnumerable<AttachmentUploadDto>> GetPresignedUrlsForUpload(IList<AttachmentUploadDto> urls);
}