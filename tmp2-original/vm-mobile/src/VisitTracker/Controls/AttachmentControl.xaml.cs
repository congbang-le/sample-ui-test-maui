namespace VisitTracker;

public partial class AttachmentControl : BaseContentView
{
    #region Commands & Constructor

    public ReactiveCommand<Unit, Unit> CameraCommand { get; set; }
    public ReactiveCommand<Unit, Unit> GalleryCommand { get; set; }
    public ReactiveCommand<Unit, Unit> RecordAudioCommand { get; set; }
    public ReactiveCommand<Unit, Unit> StopRecordingAudioCommand { get; set; }
    public ReactiveCommand<AttachmentDto, Unit> DeleteAttachmentCommand { get; set; }
    public ReactiveCommand<AttachmentDto, Unit> OpenAttachmentCommand { get; set; }

    public AttachmentControl()
    {
        InitializeComponent();

        CameraCommand = ReactiveCommand.CreateFromTask(Camera);
        GalleryCommand = ReactiveCommand.CreateFromTask(Gallery);
        RecordAudioCommand = ReactiveCommand.CreateFromTask(RecordAudio);
        StopRecordingAudioCommand = ReactiveCommand.CreateFromTask(StopRecordingAudio);
        DeleteAttachmentCommand = ReactiveCommand.CreateFromTask<AttachmentDto>(DeleteAttachment);
        OpenAttachmentCommand = ReactiveCommand.CreateFromTask<AttachmentDto>(OpenAttachment);

        BindBusy(CameraCommand);
        BindBusy(GalleryCommand);
        BindBusy(StopRecordingAudioCommand);
        BindBusy(DeleteAttachmentCommand);
        BindBusy(OpenAttachmentCommand);

        IsRecording = false;
    }

    #endregion Commands & Constructor

    #region Bindable Properties

    public static readonly BindableProperty MaxImagesProperty = BindableProperty.Create(nameof(MaxImages),
          typeof(int), typeof(AttachmentControl), defaultValue: 3);

    public static readonly BindableProperty MaxAudiosProperty = BindableProperty.Create(nameof(MaxAudios),
          typeof(int), typeof(AttachmentControl), defaultValue: 3);

    public static readonly BindableProperty AttachmentListProperty = BindableProperty.Create(nameof(AttachmentList),
            typeof(IList<AttachmentDto>), typeof(AttachmentControl),
            defaultValue: new List<AttachmentDto>(), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: AttachmentListPropertyChanged);

    public static readonly BindableProperty IsRecordingProperty = BindableProperty.Create(nameof(IsRecording),
            typeof(bool), typeof(AttachmentControl),
            defaultValue: false, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsRecordingPropertyChanged);

    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(nameof(IsPlaying),
            typeof(bool), typeof(AttachmentControl),
            defaultValue: false, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsPlayingPropertyChanged);

    public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type),
            typeof(string), typeof(AttachmentControl),
            defaultValue: null, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TypePropertyChanged);

    public static readonly BindableProperty BaseVisitIdProperty = BindableProperty.Create(nameof(BaseVisitId),
            typeof(int), typeof(AttachmentControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: BaseVisitIdPropertyChanged);

    public static readonly BindableProperty TypeIdProperty = BindableProperty.Create(nameof(TypeId),
            typeof(int), typeof(AttachmentControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TypeIdPropertyChanged);

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly),
            typeof(bool), typeof(AttachmentControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsReadOnlyPropertyChanged);

    public static void TypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (AttachmentControl)bindable;
        control.Type = newValue as string;
    }

    private static void BaseVisitIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (AttachmentControl)bindable;
        control.BaseVisitId = Convert.ToInt32(newValue);
    }

    private static void TypeIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (AttachmentControl)bindable;
        control.TypeId = Convert.ToInt32(newValue);
    }

    private static void IsReadOnlyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (AttachmentControl)bindable;
        control.IsReadOnly = (bool)newValue;
    }

    private static void AttachmentListPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (AttachmentControl)bindable;
        control.AttachmentList = newValue as IList<AttachmentDto>;
    }

    private static void IsRecordingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (AttachmentControl)bindable;
        control.IsRecording = (bool)newValue;
    }

    private static void IsPlayingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (AttachmentControl)bindable;
        control.IsPlaying = (bool)newValue;
    }

    #endregion Bindable Properties

    #region Properties

    private AttachmentDto CurrentPlayer = null;

    public int MaxImages
    {
        get
        {
            return (int)base.GetValue(MaxImagesProperty);
        }
        set
        {
            base.SetValue(MaxImagesProperty, value);
        }
    }

    public int MaxAudios
    {
        get
        {
            return (int)base.GetValue(MaxAudiosProperty);
        }
        set
        {
            base.SetValue(MaxAudiosProperty, value);
        }
    }

    public IList<AttachmentDto> AttachmentList
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

    public int BaseVisitId
    {
        get
        {
            return Convert.ToInt32(GetValue(BaseVisitIdProperty));
        }
        set
        {
            SetValue(BaseVisitIdProperty, value);
        }
    }

    public int TypeId
    {
        get
        {
            return Convert.ToInt32(GetValue(TypeIdProperty));
        }
        set
        {
            SetValue(TypeIdProperty, value);
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return (bool)GetValue(IsReadOnlyProperty);
        }
        set
        {
            SetValue(IsReadOnlyProperty, value);
        }
    }

    public string Type
    {
        get
        {
            return base.GetValue(TypeProperty) as string;
        }
        set
        {
            base.SetValue(TypeProperty, value);
        }
    }

    public bool IsRecording
    {
        get
        {
            return (bool)GetValue(IsRecordingProperty);
        }
        private set
        {
            SetValue(IsRecordingProperty, value);
        }
    }

    public bool IsNotRecording => !IsRecording;

    public bool IsPlaying
    {
        get
        {
            return (bool)GetValue(IsPlayingProperty);
        }
        private set
        {
            SetValue(IsPlayingProperty, value);
        }
    }

    public bool IsNotPlaying => !IsPlaying;

    private IAudioSource currentRecording;
    private IAudioRecorder _audioRecorder;

    #endregion Properties

    #region Events & Functions

    protected async Task Camera()
    {
        if (AttachmentList?.Count(x => x.AttachmentType == EAttachmentType.Image) >= MaxImages)
            await Application.Current.MainPage.ShowSnackbar(Messages.SummaryImageUploadMaxLimit, false, true);
        else
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            await LoadPhotoAsync(photo, "camera_" + DateTimeExtensions.NowNoTimezone().ToString("yyyyMMddHHmmssfff") + ".jpg");
        }
    }

    protected async Task Gallery()
    {
        if (AttachmentList?.Count(x => x.AttachmentType == EAttachmentType.Image) >= MaxImages)
            await Application.Current.MainPage.ShowSnackbar(Messages.SummaryImageUploadMaxLimit, false, true);
        else
        {
            var photo = await MediaPicker.PickPhotoAsync();
            await LoadPhotoAsync(photo, "gallery_" + DateTimeExtensions.NowNoTimezone().ToString("yyyyMMddHHmmssfff") + ".jpg");
        }
    }

    private async Task LoadPhotoAsync(FileResult photo, string filename)
    {
        if (photo == null)
            return;

        var newFile = Path.Combine(FileSystem.AppDataDirectory, filename);
        using (var stream = await photo.OpenReadAsync())
        using (var newStream = File.OpenWrite(newFile))
            await stream.CopyToAsync(newStream);

        var attachment = new AttachmentDto()
        {
            AttachmentType = EAttachmentType.Image,
            DisplayIcon = MaterialCommunityIconsFont.Eye,
            FileName = filename,
            FilePath = newFile
        };
        var dbAttachment = await AppServices.Current.AttachmentService.InsertOrReplace(BaseAssembler.BuildAttachment(attachment, BaseVisitId, Type, TypeId));
        attachment.Id = dbAttachment.Id;
        AttachmentList.Add(attachment);
        AttachmentList = AttachmentList.GroupBy(a => a.AttachmentType)
            .SelectMany(group => group).OrderBy(a => a.AttachmentType).ToList();
    }

    protected async Task RecordAudio()
    {
        if (AttachmentList?.Count(x => x.AttachmentType == EAttachmentType.Audio) >= MaxAudios)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.SummaryAudioUploadMaxLimit, false, true);
            return;
        }

        var status = await Permissions.RequestAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            await Application.Current.MainPage.ShowSnackbar("Microphone permission is required.", false);
            return;
        }

        try
        {
            if (_audioRecorder == null || !_audioRecorder.IsRecording)
            {
                // Start recording
                IsRecording = true;
                _audioRecorder = AppServices.Current.AudioManager.CreateRecorder();
                await _audioRecorder.StartAsync();
            }
        }
        catch (Exception ex)
        {
            IsRecording = false;
            _audioRecorder = null;
            await Application.Current.MainPage.ShowSnackbar($"Audio recording failed: {ex.Message}", false, true);
        }
    }

    protected async Task StopRecordingAudio()
    {
        if (_audioRecorder?.IsRecording == true)
        {
            // Stop recording
            currentRecording = await _audioRecorder.StopAsync();
            IsRecording = false;

            var fileName = "audio_" + DateTimeExtensions.NowNoTimezone().ToString("yyyyMMddHHmmssfff") + ".mp3";
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            // Save the audio file
            if (currentRecording != null)
            {
                using var fileStream = File.Create(filePath);
                using var audioStream = currentRecording.GetAudioStream();
                await audioStream.CopyToAsync(fileStream);

                var attachment = new AttachmentDto
                {
                    AttachmentType = EAttachmentType.Audio,
                    FileName = fileName,
                    DisplayIcon = MaterialCommunityIconsFont.PlayCircle,
                    FilePath = filePath
                };
                var dbAttachment = await AppServices.Current.AttachmentService.InsertOrReplace(BaseAssembler.BuildAttachment(attachment, BaseVisitId, Type, TypeId));
                attachment.Id = dbAttachment.Id;
                AttachmentList.Add(attachment);
                AttachmentList = AttachmentList.GroupBy(a => a.AttachmentType)
                    .SelectMany(group => group).OrderBy(a => a.AttachmentType).ToList();
            }

            // Clean up
            _audioRecorder = null;
            currentRecording = null;
        }
    }
    protected async Task DeleteAttachment(AttachmentDto attachment)
    {
        bool response = await Application.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle,
                            string.Format(Messages.DialogDeleteConfirmation, attachment.AttachmentType.ToString()), Messages.Yes, Messages.No);
        if (response)
        {
            await AppServices.Current.AttachmentService.DeleteAllById(attachment.Id);
            File.Delete(attachment.FilePath);

            AttachmentList.Remove(attachment);
            AttachmentList = AttachmentList.GroupBy(a => a.AttachmentType)
                                .SelectMany(group => group).OrderBy(a => a.AttachmentType).ToList();
        }
    }

    protected async Task OpenAttachment(AttachmentDto attachment)
    {
        await AttachmentHelper.Current.OpenAttachment(attachment, AttachmentList);
    }

    #endregion Events & Functions
}