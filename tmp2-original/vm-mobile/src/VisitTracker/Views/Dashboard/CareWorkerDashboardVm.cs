namespace VisitTracker;

public class CareWorkerDashboardVm : BaseVm, IDisposable
{
    public CareWorkerDashboardDto DashboardDto { get; set; }
    public List<NotificationsDto> Notifications { get; set; }
    public ReactiveCommand<int, Unit> BookingTapCommand { get; }
    public ReactiveCommand<NotificationsDto, Unit> NotificationTapCommand { get; }
    public ReactiveCommand<Unit, Unit> NotificationCountTapCommand { get; }

    public CareWorkerDashboardVm()
    {
        BookingTapCommand = ReactiveCommand.CreateFromTask<int>(BookingTapped);
        NotificationTapCommand = ReactiveCommand.CreateFromTask<NotificationsDto>(NotificationAction);
        NotificationCountTapCommand = ReactiveCommand.CreateFromTask(NotificationCountTap);

        BindBusyWithException(BookingTapCommand);
        BindBusyWithException(NotificationCountTapCommand);
        BindBusyWithException(NotificationTapCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        if (DashboardDto == null) DashboardDto = new CareWorkerDashboardDto();
        DashboardDto.ProfileName = AppData.Current.CurrentProfile.Name;

        var upcomingBooking = await AppServices.Current.BookingService.GetNextBooking();

        if (upcomingBooking == null)
        {
            DashboardDto.Bookings = null;
            DashboardDto.UpcomingBookingsHeight = 70;
            DashboardDto.IsVisibleNoData = true;
        }
        else
        {
            DashboardDto.Bookings = await BaseAssembler.BuildBookingsCard(new[] { upcomingBooking });
            DashboardDto.UpcomingBookingsHeight = -1;
            DashboardDto.IsVisibleNoData = false;
        }

        var TotalBookingTitle = "Bookings Statistics (Current Week)";
        DashboardDto.TotalBookingTitle = TotalBookingTitle;

        var NotificationPendingAck = await AppServices.Current.NotificationService.GetUnreadNotifications();
        DashboardDto.PendingNotificationCount = NotificationPendingAck.Count.ToString("D2");
        var PendingNotificationTitle = "Unread Notifications (" + NotificationPendingAck.Count + ")";
        DashboardDto.PendingNotificationTitle = PendingNotificationTitle;

        var dates = BaseAssembler.GetCurrentWeekDates();
        var TotalBookings = await AppServices.Current.BookingService.GetScheduledBookingsBetweenDates(
            dates.MinBy(x => x), dates.MaxBy(x => x).AddDays(1));
        var totalBookings = BaseAssembler.BuildTotalBookingsViewModel(TotalBookings);
        DashboardDto.TotalBookings = totalBookings.Item2;
        DashboardDto.CompletedBookings = totalBookings.Item1;

        var notifications = await AppServices.Current.NotificationService.GetUnreadNotifications();
        Notifications = notifications
            .Select(notification => new NotificationsDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Icon = "\uF09A",
                Color = Color.FromArgb(notification.Color),
                CreatedTime = notification.CreatedTime.Value,
                IsAcknowledged = notification.IsAcknowledged,
                Description = notification.Message,
                RequireAcknowledgement = notification.RequireAcknowledgement,
                Type = notification.NotificationType,
                TypeId = notification.TypeId,
            }).OrderBy(i => i.IsAcknowledged).ThenBy(i => i.RequireAcknowledgement).Take(1).ToList();
        await NotificationHelper.Current.OnNotificationUnreadCountChanged(NotificationPendingAck.Count);
    }

    private async Task NotificationCountTap()
    {
        await Shell.Current.GoToAsync("//notifications");
    }

    public async Task NotificationAction(NotificationsDto notification)
    {
        var dbNotification = await AppServices.Current.NotificationService.GetById(notification.Id);
        await NotificationHelper.Current.OnNotificationOpened(dbNotification, nameof(EUserType.CAREWORKER));
    }

    protected async Task BookingTapped(int Id)
    {
        var bookingDetailPage = $"{nameof(BookingDetailPage)}?Id={Id}";
        await Shell.Current.GoToAsync(bookingDetailPage);
    }

    public void SubscribeAllMessagingCenters()
    {
        var isPreVisitMonitorRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.PreVisitMonitorMessage>(this);
        if (!isPreVisitMonitorRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.PreVisitMonitorMessage>(this,
                async (recipient, message) => await InitCommand.Execute(true));
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.PreVisitMonitorMessage>(this);
    }
}