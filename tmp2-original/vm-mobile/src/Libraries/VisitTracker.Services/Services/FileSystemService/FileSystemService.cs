namespace VisitTracker.Services;

public class FileSystemService : IFileSystemService
{
    public async Task DownloadFile(Attachment attachment)
    {
        if (string.IsNullOrEmpty(attachment.S3SignedUrl) || string.IsNullOrEmpty(attachment.S3Url))
            return;

        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync(attachment.S3SignedUrl);
            if (!response.IsSuccessStatusCode)
                return;

            var filePath = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(attachment.S3Url));
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                await responseStream.CopyToAsync(fileStream);
        }
    }

    public async Task DownloadFiles(IEnumerable<Attachment> attachments)
    {
        var downloadTasks = attachments.Select(DownloadFile);
        await Task.WhenAll(downloadTasks);
    }

    public async Task UploadFile(Attachment attachment)
    {
        var filePath = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(attachment.S3Url));
        using var streamContent = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
        using (HttpClient client = new HttpClient())
        {
            var response = await client.PutAsync(attachment.S3SignedUrl, streamContent);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error uploading files!");
        }
    }

    public async Task UploadFiles(IEnumerable<Attachment> attachments)
    {
        var uploadTasks = attachments.Select(UploadFile);
        await Task.WhenAll(uploadTasks);
    }

    public async Task DeleteFile(Attachment attachment)
    {
        var filePath = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(attachment.S3Url));
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception)
            { }
        }

        await Task.CompletedTask;
    }

    public async Task DeleteFiles(IEnumerable<Attachment> attachments)
    {
        var downloadTasks = attachments.Select(DeleteFile);
        await Task.WhenAll(downloadTasks);
    }
}