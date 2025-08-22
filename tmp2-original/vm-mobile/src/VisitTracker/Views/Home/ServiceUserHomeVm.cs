namespace VisitTracker;

public class ServiceUserHomeVm : BaseVm, IDisposable
{
    public ServiceUserDashboardPage DashboardPage { get; set; }
    public BookingsPage BookingsPage { get; set; }
    public NotificationsPage NotificationsPage { get; set; }
    public MiscellaneousPage MiscellaneousPage { get; set; }

    public ServiceUserHomeVm(ServiceUserDashboardPage dashboardPage,
        BookingsPage bookingsPage,
        NotificationsPage notificationsPage,
        MiscellaneousPage miscellaneousPage)
    {
        DashboardPage = dashboardPage;
        BookingsPage = bookingsPage;
        NotificationsPage = notificationsPage;
        MiscellaneousPage = miscellaneousPage;
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        if (AppData.Current.CurrentProfile.Type != EUserType.SERVICEUSER.ToString() &&
        AppData.Current.CurrentProfile.Type != EUserType.NEXTOFKIN.ToString())
        {
            await App.Current.MainPage.DisplayAlert(Messages.Error, Messages.NoAccessPage, Messages.Ok);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            //Application.Current.Quit(); Use this if above doesn't work
        }

        await NotificationHelper.Current.SyncNotificationFromLast();
        WeakReferenceMessenger.Default.Send(new MessagingEvents.DataRetentionMessage(true));
        await NotificationHelper.Current.OnNotificationUnreadCountChanged();
    }

    public void SubscribeAllMessagingCenters()
    {
        var notificationIsRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.NotificationMessage>(this);
        if (!notificationIsRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.NotificationMessage>(this,
                async (recipient, message) =>
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var dbNotification = await AppServices.Current.NotificationService.GetById(message.Value);
                        await NotificationHelper.Current.
                        OnNotificationOpened(dbNotification);
                    });
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.NotificationMessage>(this);
    }
}