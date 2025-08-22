namespace VisitTracker;

public partial class ImageListControl : BaseContentView
{
    public ReactiveCommand<AttachmentDto, Unit> OpenAttachmentCommand { get; set; }

    public ImageListControl()
    {
        InitializeComponent();

        OpenAttachmentCommand = ReactiveCommand.CreateFromTask<AttachmentDto>(OpenAttachment);
        BindBusy(OpenAttachmentCommand);
    }

    public static readonly BindableProperty AttachmentListProperty = BindableProperty.Create(nameof(AttachmentList),
            typeof(List<AttachmentDto>), typeof(ImageListControl),
            defaultValue: new List<AttachmentDto>(), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: AttachmentListPropertyChanged);

    private static void AttachmentListPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ImageListControl)bindable;
        control.AttachmentList = newValue as List<AttachmentDto>;
    }

    public List<AttachmentDto> AttachmentList
    {
        get
        {
            return base.GetValue(AttachmentListProperty) as List<AttachmentDto>;
        }
        set
        {
            base.SetValue(AttachmentListProperty, value);
        }
    }

    protected async Task OpenAttachment(AttachmentDto attachment)
    {
        if (attachment == null || string.IsNullOrEmpty(attachment.S3Url))
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.InvalidAttachment, false, true);
            return;
        }

        var fullPath = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(attachment.S3Url));
        if (!File.Exists(fullPath))
        {
            var attachmentsToDownload = AttachmentList.Where(x => !File.Exists(Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(x.S3Url))))
                                            .Select(x => x.S3Url).ToList();
            var apiAttachments = await AppServices.Current.AttachmentService.GetSignedAttachments(attachmentsToDownload);
            await AppServices.Current.FileUploadService.DownloadFiles(apiAttachments);
            foreach (var apiAttachment in apiAttachments)
            {
                var attachmentDto = AttachmentList.FirstOrDefault(x => x.S3Url == apiAttachment.S3Url);
                apiAttachment.FileName = attachmentDto.FileName = Path.GetFileName(apiAttachment.S3Url);
                attachmentDto.FilePath = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(apiAttachment.S3Url));
            }
        }

        if (!File.Exists(fullPath))
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.NoSuchFileExists, false, true);
            return;
        }

        await Launcher.Default.OpenAsync(new OpenFileRequest("Attachment", new ReadOnlyFile(fullPath)));
    }
}