namespace VisitTracker;

/// <summary>
/// AttachmentHelper is a helper class that provides methods for downloading and uploading files, as well as opening attachments.
/// </summary>
public class AttachmentHelper
{
    public static AttachmentHelper Current => ServiceLocator.GetService<AttachmentHelper>();

    /// <summary>
    /// Downloads files from the specified list of attachments.
    /// This method checks if the attachment list is null or empty before proceeding with the download.
    /// </summary>
    /// <param name="attachmentList"></param>
    /// <returns></returns>
    public async Task DownloadFiles(IList<Attachment> attachmentList)
    {
        if (attachmentList == null || !attachmentList.Any()) return;
        await AppServices.Current.FileUploadService.DownloadFiles(attachmentList);
    }

    /// <summary>
    /// Uploads files to the specified folder path using the provided booking unique ID.
    /// This method generates S3 URLs for each attachment and retrieves presigned URLs for upload.
    /// </summary>
    /// <param name="attachmentList"></param>
    /// <param name="folderPath"></param>
    /// <param name="bookingUniqueId"></param>
    /// <returns></returns>
    public async Task UploadFiles(IList<Attachment> attachmentList, string folderPath, string bookingUniqueId)
    {
        attachmentList.ToList().ForEach(i => i.S3Url = Constants.S3BucketAppPrefix + "/" + folderPath + "/" + bookingUniqueId + "/" + i.FileName);

        var signedUploadAttachments = await AppServices.Current.AttachmentService.GetPresignedUrlsForUpload(
            attachmentList.Select(x => new AttachmentUploadDto
            {
                Id = x.Id,
                Key = x.S3Url
            }).ToList()
        );

        foreach (var signedUrl in signedUploadAttachments)
        {
            var attachment = attachmentList.FirstOrDefault(x => x.Id == signedUrl.Id);
            attachment.FileName = attachment.FileName;
            attachment.S3SignedUrl = signedUrl.S3SignedUrl;
            attachment.S3Url = attachment.S3Url;
            await AppServices.Current.AttachmentService.InsertOrReplace(attachment);
        }

        await AppServices.Current.FileUploadService.UploadFiles(attachmentList);
    }

    /// <summary>
    /// Opens the specified attachment using the default file launcher.
    /// This method checks if the attachment is null or if the file path is invalid before attempting to open it.
    /// </summary>
    /// <param name="attachment"></param>
    /// <param name="attachmentList"></param>
    /// <returns></returns>
    public async Task OpenAttachment(AttachmentDto attachment, IList<AttachmentDto> attachmentList)
    {
        if (attachment == null)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.InvalidAttachment, false, true);
            return;
        }

        if (string.IsNullOrEmpty(attachment.FilePath) || !File.Exists(Path.Combine(attachment.FilePath)))
        {
            var attachmentsToDownload = attachmentList
                .Where(x => !File.Exists(Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(x.FileName))))
                .Select(x => x.ServerRef)
                .ToList();

            var apiAttachments = await AppServices.Current.AttachmentService.GetSignedAttachments(attachmentsToDownload);
            await AppServices.Current.FileUploadService.DownloadFiles(apiAttachments);

            foreach (var apiAttachment in apiAttachments)
            {
                var attachmentDto = attachmentList.FirstOrDefault(x => x.ServerRef == apiAttachment.ServerRef);
                if (attachmentDto != null)
                {
                    apiAttachment.FileName = attachmentDto.FileName = Path.GetFileName(apiAttachment.S3Url);
                    attachmentDto.FilePath = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(apiAttachment.S3Url));
                    apiAttachment.Id = attachmentDto.Id;
                    await AppServices.Current.AttachmentService.InsertOrReplace(apiAttachment);
                }
            }
        }

        await Launcher.Default.OpenAsync(new OpenFileRequest("Attachment", new ReadOnlyFile(attachment.FilePath)));
    }
}