namespace VisitTracker;

public class NotificationsVm : BaseVm
{
    public List<NotificationsDto> Notifications { get; set; }
    public int UnreadNotificationsCount { get; set; }
    public bool IsRefreshing { get; set; }

    private bool _isToggled;
    public bool IsToggled
    {
        get => _isToggled;
        set
        {
            this.RaiseAndSetIfChanged(ref _isToggled, value);
            ToggleCommand.Execute(value).Subscribe();
        }
    }

    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<bool, Unit> ToggleCommand { get; }
    public ReactiveCommand<NotificationsDto, Unit> NotificationTapCommand { get; }

    public NotificationsVm()
    {
        RefreshCommand = ReactiveCommand.CreateFromTask(OnRefresh);
        NotificationTapCommand = ReactiveCommand.CreateFromTask<NotificationsDto>(NotificationAction);
        ToggleCommand = ReactiveCommand.CreateFromTask<bool>(OnNotificationToggle);

        BindBusyWithException(RefreshCommand);
        BindBusyWithException(NotificationTapCommand);
        BindBusyWithException(ToggleCommand);
    }

    protected override async Task Init()
    {
        _isToggled = false;
        this.RaisePropertyChanged(nameof(IsToggled));

        var notifications = await AppServices.Current.NotificationService.GetAll();
        Notifications = MapNotifications(notifications, false);
        UnreadNotificationsCount = Notifications.Count(x => !x.IsAcknowledged);

        await NotificationHelper.Current.OnNotificationUnreadCountChanged(UnreadNotificationsCount);
    }

    public async Task OnNotificationToggle(bool isToggled)
    {
        var notifications = await AppServices.Current.NotificationService.GetAll();
        Notifications = MapNotifications(notifications, isToggled);
        if (!isToggled)
        {
            UnreadNotificationsCount = Notifications.Count(x => !x.IsAcknowledged);
        }
    }

    private List<NotificationsDto> MapNotifications(IEnumerable<VisitTracker.Domain.Notification> notifications, bool onlyAcknowledged)
    {
        var filteredNotifications = notifications
            .Where(notification => onlyAcknowledged ? notification.IsAcknowledged : !notification.IsAcknowledged)
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
            })
            .OrderByDescending(i => i.CreatedTime)
            .ThenBy(i => i.RequireAcknowledgement)
            .ToList();

        return filteredNotifications;
    }

    protected async Task OnRefresh()
    {
        //await AppServices.Current.NotificationService.SyncAll();
        //TODO: following is a temporary fix. Should not do sync all as the call back wont be triggered
        await NotificationHelper.Current.SyncNotificationFromLast();

        await InitCommand.Execute(true);
        IsRefreshing = false;
    }

    public async Task NotificationAction(NotificationsDto notification)
    {
        var dbNotification = await AppServices.Current.NotificationService.GetById(notification.Id);
        await NotificationHelper.Current.OnNotificationOpened(dbNotification);
    }
}