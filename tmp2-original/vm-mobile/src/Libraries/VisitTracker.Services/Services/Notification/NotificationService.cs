namespace VisitTracker.Services;

public class NotificationService : BaseService<Notification>, INotificationService
{
    private readonly INotificationApi _api;
    private readonly INotificationStorage _storage;

    public NotificationService(INotificationApi notificationApi,
        INotificationStorage notificationStorage) : base(notificationStorage)
    {
        _api = notificationApi;
        _storage = notificationStorage;
    }

    public async Task<IList<Notification>> GetUnreadNotifications()
    {
        return await _storage.GetUnreadNotifications();
    }

    public async Task<Notification> MarkAsRead(int notificationId)
    {
        return await _storage.MarkAsRead(notificationId);
    }

    public async Task<IList<Notification>> SyncAll()
    {
        var notifications = await _api.GetAllNotifications();
        if (notifications != null)
            return await _storage.InsertOrReplace(notifications);

        return notifications;
    }

    public async Task<IList<Notification>> SyncByLastId(int id)
    {
        var notifications = await _api.GetNotificationsByLastId(id);
        if (notifications != null)
            return await _storage.InsertOrReplace(notifications);

        return notifications;
    }
}