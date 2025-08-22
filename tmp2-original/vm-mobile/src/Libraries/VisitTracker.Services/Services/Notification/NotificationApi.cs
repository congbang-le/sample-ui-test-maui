namespace VisitTracker.Services;

public class NotificationApi : INotificationApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public NotificationApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<List<Notification>> GetAllNotifications()
    {
        return await _requestProvider.ExecuteAsync<List<Notification>>(
            Constants.EndUrlGetNotification, HttpMethod.Get
        );
    }

    public async Task<List<Notification>> GetNotificationsByLastId(int id)
    {
        return await _requestProvider.ExecuteAsync<List<Notification>>(
            Constants.EndUrlNotificationGetFromLastId, HttpMethod.Post, id
        );
    }
}