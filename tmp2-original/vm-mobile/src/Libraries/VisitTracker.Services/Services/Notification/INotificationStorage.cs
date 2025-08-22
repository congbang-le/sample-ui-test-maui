namespace VisitTracker.Services;

public interface INotificationStorage : IBaseStorage<Notification>
{
    Task<Notification> MarkAsRead(int notificationId);

    Task<IList<Notification>> GetUnreadNotifications();
}