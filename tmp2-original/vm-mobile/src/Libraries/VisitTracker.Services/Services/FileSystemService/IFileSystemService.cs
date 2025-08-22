namespace VisitTracker.Services;

public interface IFileSystemService
{
    Task DownloadFile(Attachment attachment);

    Task DownloadFiles(IEnumerable<Attachment> attachments);

    Task UploadFile(Attachment attachment);

    Task UploadFiles(IEnumerable<Attachment> attachments);

    Task DeleteFile(Attachment attachment);

    Task DeleteFiles(IEnumerable<Attachment> attachments);
}