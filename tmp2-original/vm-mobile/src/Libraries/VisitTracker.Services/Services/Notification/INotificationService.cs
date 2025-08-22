namespace VisitTracker.Services;

public interface INotificationService : IBaseService<Notification>
{
    Task<IList<Notification>> GetUnreadNotifications();

    Task<Notification> MarkAsRead(int notificationId);

    Task<IList<Notification>> SyncAll();

    Task<IList<Notification>> SyncByLastId(int id);
}