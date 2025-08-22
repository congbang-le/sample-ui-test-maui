namespace VisitTracker;

public class ServiceUserDashboardVm : BaseVm, IDisposable
{
    public ServiceUserDashboardDto DashboardDto { get; set; }
    public List<NotificationsDto> Notifications { get; set; }
    public ReactiveCommand<NotificationsDto, Unit> NotificationTapCommand { get; }
    public ReactiveCommand<Unit, Unit> NotificationCountTapCommand { get; }

    public ServiceUserDashboardVm()
    {
        NotificationTapCommand = ReactiveCommand.CreateFromTask<NotificationsDto>(NotificationAction);
        NotificationCountTapCommand = ReactiveCommand.CreateFromTask(NotificationCountTap);

        BindBusyWithException(NotificationCountTapCommand);
        BindBusyWithException(NotificationTapCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        if (DashboardDto == null)
            DashboardDto = new ServiceUserDashboardDto();

        var upcomingBooking = await AppServices.Current.BookingService.GetNextBooking();
        if (upcomingBooking == null)
        {
            DashboardDto.Bookings = null;
            DashboardDto.UpcomingBookingsHeight = 70;
            DashboardDto.IsVisibleNoData = true;
        }
        else
        {
            DashboardDto.Bookings = await BaseAssembler.BuildBookingsCard([upcomingBooking]);
            DashboardDto.UpcomingBookingsHeight = -1;
            DashboardDto.IsVisibleNoData = false;
        }

        DashboardDto.ProfileName = AppData.Current.CurrentProfile.Name;
        var notificationPendingAck = await AppServices.Current.NotificationService.GetUnreadNotifications();
        DashboardDto.PendingNotificationCount = notificationPendingAck.Count.ToString("D2");
        DashboardDto.TotalBookingTitle = "Bookings Statistics (Current Week)";

        var dates = BaseAssembler.GetCurrentWeekDates();
        var bookings = await AppServices.Current.BookingService.GetScheduledBookingsBetweenDates(
            dates.MinBy(x => x), dates.MaxBy(x => x).AddDays(1));
        DashboardDto.CompletedBookings = BaseAssembler.BuildCompletedBookingsCountViewModel(bookings);
        DashboardDto.TotalBookings = BaseAssembler.BuildTotalDayBookingsViewModel(bookings);

        var notifications = await AppServices.Current.NotificationService.GetUnreadNotifications();
        Notifications = notifications.Select(notification => new NotificationsDto
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
    }

    private async Task NotificationCountTap()
    {
        await Shell.Current.GoToAsync("//notifications");
    }

    public async Task NotificationAction(NotificationsDto notification)
    {
        var dbNotification = await AppServices.Current.NotificationService.GetById(notification.Id);
        await NotificationHelper.Current.OnNotificationOpened(dbNotification, nameof(EUserType.SERVICEUSER));
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