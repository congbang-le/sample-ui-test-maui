namespace VisitTracker.Services;

public class NotificationStorage : BaseStorage<Notification>, INotificationStorage
{
    public NotificationStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<Notification> MarkAsRead(int notificationId)
    {
        var notification = await GetById(notificationId);
        notification.IsAcknowledged = true;
        notification.AcknowledgedTime = DateTimeExtensions.NowNoTimezone();
        return await InsertOrReplace(notification);
    }

    public async Task<IList<Notification>> GetUnreadNotifications()
    {
        return await Select(q => q.Where(x => !x.IsAcknowledged));
    }
}