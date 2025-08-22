namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
public class CareWorkerDetailVm : BaseVm, IQueryAttributable, IDisposable
{
    public int Id { get; set; }
    public CareWorker CareWorker { get; set; }
    public IList<BookingCardDto> Bookings { get; set; }
    public IList<IncidentResponseDto> Incidents { get; set; }
    public bool IsIncidentNotAvailable { get; set; }
    public ReactiveCommand<object, Unit> TabChangeCommand { get; }

    private DateTime? bookingsDate;

    public DateTime? BookingsDate
    {
        get => bookingsDate;
        set
        {
            if (bookingsDate != value)
            {
                bookingsDate = value;
                Bookings = null;

                if (bookingsDate.HasValue && OnBookingsDateChangedCommand.CanExecute.FirstAsync().Wait())
                {
                    OnBookingsDateChangedCommand.Execute().Subscribe();
                }
            }
        }
    }

    public string RosterTabText { get; set; } = "Rosters";
    public string IncidentTabText { get; set; } = "Incidents";
    public bool IsBookingNotAvailable { get; set; } = false;

    public DateTime BookingsMinDate { get; set; }
        = DateTimeExtensions.NowNoTimezone().AddDays(-Constants.NoOfDaysBookingIncidentToPick);

    public DateTime BookingMaxDate { get; set; }
    = DateTimeExtensions.NowNoTimezone();


    public double CardViewHeight { get; set; }
    public ReactiveCommand<IncidentResponseDto, Unit> OpenIncidentReportCommand { get; }
    public ReactiveCommand<Unit, Unit> OnBookingsDateChangedCommand { get; }
    public ReactiveCommand<string, Unit> CallCommand { get; }
    public CareWorkerDetailVm()
    {
        OpenIncidentReportCommand = ReactiveCommand.CreateFromTask<IncidentResponseDto>(OpenIncidentReport);
        CallCommand = ReactiveCommand.CreateFromTask<string>(OnCall);
        OnBookingsDateChangedCommand = ReactiveCommand.CreateFromTask(OnBookingsDateChanged);
        TabChangeCommand = ReactiveCommand.CreateFromTask<object>(OnTabChange);

        BindBusyWithException(OpenIncidentReportCommand);
        BindBusyWithException(CallCommand);
        BindBusyWithException(OnBookingsDateChangedCommand);
        BindBusyWithException(TabChangeCommand);
    }

    private async Task OnTabChange(object parameter)
    {
        if (parameter is UraniumUI.Material.Controls.TabItem tabItem)
        {
            var newTabTitle = tabItem.Title;
            if (newTabTitle == IncidentTabText)
            {
                Incidents = null;
                Incidents = await AppServices.Current.IncidentService.GetAllByCareWorker(CareWorker.Id);

                if (Incidents == null || !Incidents.Any())
                {
                    Incidents = null;
                    IsIncidentNotAvailable = true;
                    return;
                }
                else { 
                    IsIncidentNotAvailable = false;
                    return;
                }
                   
            }
        }
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();
        SystemHelper.Current.HideKeyboard();

        if (AppData.Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER)
            && AppData.Current.CurrentProfile?.Id != Id)
        {
            await Shell.Current.Navigation.PopAsync();
            await Application.Current.MainPage.ShowSnackbar(Messages.NoAccessPage, false);
            return;
        }

        CareWorker = await AppServices.Current.CareWorkerService.GetById(Id);
        Title = CareWorker.Name;
        var currentDateTime = DateTimeExtensions.NowNoTimezone();
        BookingsDate = AppData.Current.CurrentProfile.Type == EUserType.SUPERVISOR.ToString()
            ? currentDateTime : currentDateTime.Date.AddDays(-Constants.DefaultBookingPastDays);

        Incidents = null;
        Incidents = await AppServices.Current.IncidentService.GetAllByCareWorker(CareWorker.Id);

        Bookings = null;

        CardViewHeight = SystemHelper.Current.SetCardViewHeight(350);
    }

    private async Task OnBookingsDateChanged()
    {
        if (!BookingsDate.HasValue)
            return;

        var bookings = await AppServices.Current.BookingService.GetAllByCwAndDate(CareWorker.Id, BookingsDate.Value);
        if (bookings == null || !bookings.Any())
        {
            Bookings = null;
            IsBookingNotAvailable = true;
            return;
        }

        Bookings = await BaseAssembler.BuildBookingsCard(bookings);
        IsBookingNotAvailable = false;
    }

    protected async Task OnCall(string phone)
    {
        await SystemHelper.Current.Open(phone);
    }

    protected async Task OpenIncidentReport(IncidentResponseDto incidentDto)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            {"OnlineIncidentId", incidentDto.Id },
            {"IsReadOnly", true }
        };

        var IncidentReportDetailpage = $"{nameof(IncidentReportPage)}?";
        await Shell.Current.GoToAsync(IncidentReportDetailpage, navigationParameter);
    }

    public void SubscribeAllMessagingCenters()
    {
        var cwBookingEditCompletedRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.CareWorkerDetailPageBookingEditCompleted>(this);
        if (!cwBookingEditCompletedRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.CareWorkerDetailPageBookingEditCompleted>(this,
                async (recipient, message) => await InitCommand.Execute(true));
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.CareWorkerDetailPageBookingEditCompleted>(this);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var param))
            Id = Convert.ToInt32(param.ToString());

        await InitCommand.Execute();
    }
}