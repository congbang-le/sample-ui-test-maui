namespace VisitTracker.Services;

public interface INotificationApi
{
    Task<List<Notification>> GetAllNotifications();

    Task<List<Notification>> GetNotificationsByLastId(int id);
}