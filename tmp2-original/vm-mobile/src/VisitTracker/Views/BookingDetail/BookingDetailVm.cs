using System.Globalization;

namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
[QueryProperty(nameof(VisitId), nameof(VisitId))]
[QueryProperty(nameof(NotificationMessage), nameof(NotificationMessage))]
public class BookingDetailVm : BaseVm, IQueryAttributable
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public string NotificationMessage { get; set; }

    public BookingDetailDto BookingDetailDto { get; set; }

    public bool ShowHandOverNotesButton { get; set; } = true;
    public string HandOverNotes { get; set; }

    [Reactive] public bool EnableEditButton { get; set; }

    public string TypeName => EPageType.BookingType.ToString();

    public ReactiveCommand<int, Unit> OpenTaskViewCommand { get; }
    public ReactiveCommand<int, Unit> OpenMedicationViewCommand { get; }
    public ReactiveCommand<int, Unit> OpenFluidIntakeViewCommand { get; }
    public ReactiveCommand<Unit, Unit> DownloadHandOverNotesCommand { get; }
    private bool _isHandOverNotesExpanded;
    private bool _hasHandOverNoteDownloaded;
    public ReactiveCommand<Unit, Unit> CheckEditAccessCommand { get; }

    private ImageSource _toolbarIconSource;
    public ImageSource ToolbarIconSource
    {
        get
        {
            if (BookingDetailDto?.IsVisited == true)
            {
                return new FontImageSource
                {
                    FontFamily = "MaterialCommunityIcons",
                    Glyph = "\uF90B",
                    Size = 28,
                    Color= Colors.White              
                };
            }
            else
            {
                return (ImageSource)new ToolbarIconConvertor().Convert(new object[] { EnableEditButton }, typeof(ImageSource), null, CultureInfo.InvariantCulture);
            }
        }
    }
    public bool IsHandOverNotesExpanded
    {
        get => _isHandOverNotesExpanded;
        set
        {
            this.RaiseAndSetIfChanged(ref _isHandOverNotesExpanded, value);
            if (_isHandOverNotesExpanded && !_hasHandOverNoteDownloaded)
            {
                _hasHandOverNoteDownloaded = true;
                DownloadHandOverNotesCommand?.Execute(Unit.Default);
            }
        }
    }

    public BookingDetailVm()
    {
        OpenTaskViewCommand = ReactiveCommand.CreateFromTask<int>(OpenTaskView);
        OpenMedicationViewCommand = ReactiveCommand.CreateFromTask<int>(OpenMedicationView);
        OpenFluidIntakeViewCommand = ReactiveCommand.CreateFromTask<int>(OpenFluidView);
        DownloadHandOverNotesCommand = ReactiveCommand.CreateFromTask(DownloadHandOverNotes);
        CheckEditAccessCommand = ReactiveCommand.CreateFromTask(CheckEditAccess);

        BindBusyWithException(OpenTaskViewCommand);
        BindBusyWithException(OpenMedicationViewCommand);
        BindBusyWithException(OpenFluidIntakeViewCommand);
        BindBusyWithException(DownloadHandOverNotesCommand);
        BindBusyWithException(CheckEditAccessCommand);
    }

    protected override async Task Init()
    {
        IsReadOnly = false;

        var booking = await AppServices.Current.BookingService.GetById(Id);
        if (booking == null)
        {
            await AppServices.Current.BookingService.SyncBookingById(Id);
            booking = await AppServices.Current.BookingService.GetById(Id);
        }

        Visit Visit = null;
        if (VisitId != default)
            Visit = await AppServices.Current.VisitService.GetById(VisitId);

        BookingDetailDto = await BaseAssembler.BuildBookingDetailViewModels(booking, Visit);

        if (!string.IsNullOrEmpty(NotificationMessage))
            await Application.Current.MainPage.DisplayAlert(Messages.Notification, NotificationMessage, Messages.Ok);

        HandOverNotes = BookingDetailDto.Visit?.HandOverNotes;

        EnableEditButton = AppData.Current.CurrentProfile.Type == EUserType.CAREWORKER.ToString()
            || AppData.Current.CurrentProfile.Type == EUserType.SUPERVISOR.ToString();
    }

    protected async Task DownloadHandOverNotes()
    {
        BookingDetailDto.HandOverNotes = await AppServices.Current.BookingService.UpdateHandOverNotes(BookingDetailDto.Id);
        ShowHandOverNotesButton = false;
    }

    protected async Task CheckEditAccess()
    {
        var editAllowed = await AppServices.Current.BookingService.CheckBookingEditAccess(Id);

        if (editAllowed == null || !editAllowed.CanEdit || editAllowed.Visit == null)
        {
            await App.Current.MainPage.DisplayAlert(Messages.Message, Messages.BookingEditNotAllowed, Messages.Ok);
            return;
        }

        if (VisitId == default || VisitId == editAllowed.Visit.Id)
        {
            var visitIdToEdit = VisitId;
            if (editAllowed.Visit != null)
            {
                editAllowed.Visit = await AppServices.Current.VisitService.InsertOrReplace(editAllowed.Visit);
                visitIdToEdit = editAllowed.Visit.Id;
            }

            await Shell.Current.Navigation.PopAsync(false);

            var navigationParameter = new Dictionary<string, object> {
                { nameof(Id), Id } ,
                { nameof(VisitId), visitIdToEdit }
            };
            var bookingEditPage = $"{nameof(BookingEditPage)}?";
            await Shell.Current.GoToAsync(bookingEditPage, navigationParameter);
        }
        else
        {
            await App.Current.MainPage.DisplayAlert(Messages.Message, Messages.CanOnlyEditLatestVisitReport, Messages.Ok);
            return;
        }
    }

    protected async Task OpenTaskView(int taskId)
    {
        if (BookingDetailDto?.Visit?.Id == null)
            return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Id", taskId },
            { "BaseVisitId", BookingDetailDto.Visit.Id },
            { "IsReadOnly", true }
        };

        var taskDetailViewPage = $"{nameof(TaskDetailPage)}?";
        await Shell.Current.GoToAsync(taskDetailViewPage, navigationParameter);
    }

    protected async Task OpenMedicationView(int medicationId)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Id", medicationId },
            { "BaseVisitId", BookingDetailDto.Visit?.Id ?? 0 },
            { "IsReadOnly", true }
        };

        var medicationDetailViewPage = $"{nameof(MedicationDetailPage)}?";
        await Shell.Current.GoToAsync(medicationDetailViewPage, navigationParameter);
    }

    protected async Task OpenFluidView(int fluidId)
    {
        if (BookingDetailDto?.Visit?.Id == null)
            return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Id", fluidId },
            { "BaseVisitId", BookingDetailDto.Visit.Id },
            { "BookingId", Id },
            { "IsReadOnly", true }
        };

        var fluidDetailViewPage = $"{nameof(FluidDetailPage)}?";
        await Shell.Current.GoToAsync(fluidDetailViewPage, navigationParameter);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var id))
            Id = Convert.ToInt32(id.ToString());
        if (query.TryGetValue(nameof(VisitId), out var visitId))
            VisitId = Convert.ToInt32(visitId.ToString());
        if (query.TryGetValue(nameof(NotificationMessage), out var notificationMessage))
            NotificationMessage = notificationMessage.ToString();

        await InitCommand.Execute();
    }
}