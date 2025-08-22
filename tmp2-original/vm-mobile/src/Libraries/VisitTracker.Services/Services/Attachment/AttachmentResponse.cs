namespace VisitTracker.Services;

public class AttachmentUploadDto
{
    public int Id { get; set; }

    public string Key { get; set; }

    public string S3SignedUrl { get; set; }
}