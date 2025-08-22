namespace VisitTracker;

public class CareWorkerHomeVm : BaseVm, IDisposable, INotifyPropertyChanged
{
    public CareWorkerDashboardPage DashboardPage { get; set; }
    public BookingsPage BookingsPage { get; set; }
    public OngoingPage OngoingPage { get; set; }
    public NotificationsPage NotificationsPage { get; set; }
    public MiscellaneousPage MiscellaneousPage { get; set; }

    private string _tabTitle;
    public string TabTitle
    {
        get => _tabTitle;
        set
        {
            if (_tabTitle != value)
            {
                _tabTitle = value;
                OnPropertyChanged(nameof(TabTitle));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public Timer autoSensorTimer { get; set; }

    public CareWorkerHomeVm(CareWorkerDashboardPage dashboardPage,
        BookingsPage bookingsPage,
        OngoingPage ongoingPage,
        NotificationsPage notificationsPage,
        MiscellaneousPage miscellaneousPage)
    {
        DashboardPage = dashboardPage;
        BookingsPage = bookingsPage;
        OngoingPage = ongoingPage;
        NotificationsPage = notificationsPage;
        MiscellaneousPage = miscellaneousPage;
        TabTitle = "Upcoming";

    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        if (AppData.Current.CurrentProfile.Type != EUserType.CAREWORKER.ToString())
        {
            await App.Current.MainPage.DisplayAlert(Messages.Error, Messages.NoAccessPage, Messages.Ok);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            //Application.Current.Quit(); Use this if above doesn't work
        }

        await AutoInitializeSensingLogic();

        InitiateAutoSensorTimer();

        await NotificationHelper.Current.SyncNotificationFromLast();
        WeakReferenceMessenger.Default.Send(new MessagingEvents.DataRetentionMessage(true));
        await NotificationHelper.Current.OnNotificationUnreadCountChanged();
    }

    public void SubscribeAllMessagingCenters()
    {
        var pageTabTitleChangeIsRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.PageTabTitleChangeMessage>(this);
        if (!pageTabTitleChangeIsRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.PageTabTitleChangeMessage>(this, 
                async (recipient, message) =>
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        TabTitle = message.Value;
                    });
                });

        var notificationIsRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.NotificationMessage>(this);
        if (!notificationIsRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.NotificationMessage>(this,
                async (recipient, message) =>
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var dbNotification = await AppServices.Current.NotificationService.GetById(message.Value);
                        if (dbNotification != null)
                            await NotificationHelper.Current.OnNotificationOpened(dbNotification);
                    });
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.NotificationMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.PageTabTitleChangeMessage>(this);
    }

    private async Task AutoInitializeSensingLogic()
    {
        var errors = await App.FeaturePermissionHandlerService.CheckEverything();
        if (errors != null && errors.Any())
        {
            await OpenActions.OpenErrorPage();
            return;
        }

        var IsTracking = Preferences.Default.Get(Constants.PrefKeyLocationUpdates, false);
        if (!IsTracking)
        {
            Booking booking = null;
            if (Preferences.Default.ContainsKey(Constants.PrefKeyOngoingNonMasterCwBookingId))
            {
                var bookingId = Preferences.Default.Get<int>(Constants.PrefKeyOngoingNonMasterCwBookingId, default);
                if (bookingId != default)
                    booking = await AppServices.Current.BookingService.GetById(bookingId);
            }

            if (booking == null)
                booking = await AppServices.Current.BookingService.GetCurrentBooking() ?? await AppServices.Current.BookingService.GetNextBooking();
            if (booking != null && booking.IsBookingInScope)
            {
                var bookingDetail = await AppServices.Current.BookingDetailService.GetBookingDetailForCurrentCw(booking.Id);
                await AppServices.Current.CareWorkerTrackerService.StartNormalModeByAck(bookingDetail.Id);

                var ongoingVm = ServiceLocator.GetService<OngoingVm>();
                if (ongoingVm?.OngoingDto?.Booking == null)
                    await ongoingVm.InitCommand.Execute(true);
            }
        }
    }

    private void InitiateAutoSensorTimer()
    {
        if (autoSensorTimer == null)
            autoSensorTimer = new Timer();

        autoSensorTimer.Interval = Constants.AutoSensingTimerIntervalInMs;
        autoSensorTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            MainThread.InvokeOnMainThreadAsync(async () => await AutoInitializeSensingLogic());
        autoSensorTimer.Start();
    }
}