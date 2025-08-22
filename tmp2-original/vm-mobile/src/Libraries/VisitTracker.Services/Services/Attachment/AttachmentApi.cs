namespace VisitTracker.Services;

public class AttachmentApi : IAttachmentApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public AttachmentApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<IList<Attachment>> GetSignedAttachments(IList<int> attachmentIds)
    {
        return await _requestProvider.ExecuteAsync<IList<Attachment>>(
                    Constants.EndUrlGetSignedAttachments, HttpMethod.Post,
                    attachmentIds
                );
    }

    public async Task<IList<Attachment>> GetSignedAttachments(IList<string> attachmentUrls)
    {
        return await _requestProvider.ExecuteAsync<IList<Attachment>>(
                    Constants.EndUrlGetSignedAttachmentsByUrl, HttpMethod.Post,
                    attachmentUrls
                );
    }

    public async Task<IEnumerable<AttachmentUploadDto>> GetPresignedUrlsForUpload(IList<AttachmentUploadDto> Urls)
    {
        return await _requestProvider.ExecuteAsync<IEnumerable<AttachmentUploadDto>>(
            Constants.EndUrlGetPresignedUrlsForUpload, HttpMethod.Post, Urls
        );
    }
}