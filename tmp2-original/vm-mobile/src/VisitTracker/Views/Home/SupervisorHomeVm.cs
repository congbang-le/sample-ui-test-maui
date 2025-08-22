namespace VisitTracker;

public class SupervisorHomeVm : BaseVm, IDisposable
{
    public SupervisorDashboardPage SupervisorDashboardPage { get; set; }
    public ServiceUsersPage ServiceUsersPage { get; set; }
    public CareWorkersPage CareWorkersPage { get; set; }
    public NotificationsPage NotificationsPage { get; set; }
    public MiscellaneousPage MiscellaneousPage { get; set; }

    public Timer autoSensorTimer { get; set; }

    public SupervisorHomeVm(ServiceUsersPage serviceUsersPage,
        SupervisorDashboardPage supervisorDashboardPage,
        CareWorkersPage careWorkersPage,
        NotificationsPage notificationsPage,
        MiscellaneousPage miscellaneousPage)
    {
        ServiceUsersPage = serviceUsersPage;
        CareWorkersPage = careWorkersPage;
        SupervisorDashboardPage = supervisorDashboardPage;
        NotificationsPage = notificationsPage;
        MiscellaneousPage = miscellaneousPage;
    }

    protected override async Task Init()
    {
        if (AppData.Current.CurrentProfile.Type != EUserType.SUPERVISOR.ToString())
        {
            await App.Current.MainPage.DisplayAlert(Messages.Error, Messages.NoAccessPage, Messages.Ok);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            //Application.Current.Quit(); Use this if above doesn't work
        }

        var notificationIsRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.NotificationMessage>(this);
        if (!notificationIsRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.NotificationMessage>(this,
                async (recipient, message) =>
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var dbNotification = await AppServices.Current.NotificationService.GetById(message.Value);
                        await NotificationHelper.Current.OnNotificationOpened(dbNotification);
                    });
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.NotificationMessage>(this);
    }
}