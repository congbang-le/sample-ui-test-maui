namespace VisitTracker;

public class BookingsVm : BaseVm, IDisposable
{
    public List<BookingCardDto> Bookings { get; set; }

    public DateTime LastDisplayDate { get; set; }
    public DateTime DisplayDate { get; set; }
    public DateTime FirstDisplayDate { get; set; }
    public bool IsToday => DisplayDate.Date == DateTimeExtensions.NowNoTimezone().Date.Date;

    public bool ShowPastReportsView { get; set; }

    public int? RosterId { get; set; }

    public ReactiveCommand<Unit, Unit> NextDayCommand { get; }
    public ReactiveCommand<Unit, Unit> PrevDayCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenPastReportsCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    public BookingsVm()
    {
        PrevDayCommand = ReactiveCommand.CreateFromTask(MovePrevDay);
        NextDayCommand = ReactiveCommand.CreateFromTask(MoveNextDay);
        OpenPastReportsCommand = ReactiveCommand.CreateFromTask(OpenPastReports);
        RefreshCommand = ReactiveCommand.CreateFromTask(OnRefresh);

        BindBusyWithException(RefreshCommand);
        BindBusyWithException(NextDayCommand);
        BindBusyWithException(PrevDayCommand);
        BindBusyWithException(OpenPastReportsCommand);

        LastDisplayDate = DateTimeExtensions.NowNoTimezone().AddDays(Constants.NoOfDaysBookingsToShow).Date;
        DisplayDate = DateTimeExtensions.NowNoTimezone().Date;
        FirstDisplayDate = DateTimeExtensions.NowNoTimezone().AddDays(-Constants.NoOfDaysBookingsToShow).Date;
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        if (RosterId.HasValue)
        {
            var minDate = await AppServices.Current.BookingService.GetMinDateByRosterId(RosterId.Value);
            if (minDate.HasValue) DisplayDate = minDate.Value;
            RosterId = null;
        }

        ShowPastReportsView = AppData.Current.CurrentProfile.Type == EUserType.CAREWORKER.ToString()
            || AppData.Current.CurrentProfile.Type == EUserType.SERVICEUSER.ToString()
            || AppData.Current.CurrentProfile.Type == EUserType.NEXTOFKIN.ToString();

        var bookings = await AppServices.Current.BookingService.GetAllBookingsByDate(DisplayDate);
        if (bookings == null || !bookings.Any())
        {
            await Task.Delay(200);
            Bookings = null;
            return;
        }

        Bookings = await BaseAssembler.BuildBookingsCard(bookings);
    }

    protected async Task MovePrevDay()
    {
        if (DisplayDate != FirstDisplayDate)
            DisplayDate = DisplayDate.AddDays(-1);

        await InitCommand.Execute(true);
    }

    protected async Task MoveNextDay()
    {
        if (DisplayDate != LastDisplayDate)
            DisplayDate = DisplayDate.AddDays(1);

        await InitCommand.Execute(true);
    }

    protected async Task OnRefresh() => await InitCommand.Execute(true);

    protected async Task OpenPastReports()
    {
        var detailPage = AppData.Current.CurrentProfile.Type switch
        {
            nameof(EUserType.NEXTOFKIN) or nameof(EUserType.SERVICEUSER)
                => $"{nameof(ServiceUserDetailPage)}?Id={AppData.Current.CurrentProfile.Id}",
            nameof(EUserType.CAREWORKER) => $"{nameof(CareWorkerDetailPage)}?Id={AppData.Current.CurrentProfile.Id}",
            _ => string.Empty
        };

        if (detailPage == string.Empty) return;
        await Shell.Current.GoToAsync(detailPage);
    }

    public void SubscribeAllMessagingCenters()
    {
        var isPreVisitMonitorRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.PreVisitMonitorMessage>(this);
        if (!isPreVisitMonitorRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.PreVisitMonitorMessage>(this,
                async (recipient, message) =>
                {
                    if (message.Value)
                        await InitCommand.Execute(true);
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.PreVisitMonitorMessage>(this);
    }
}