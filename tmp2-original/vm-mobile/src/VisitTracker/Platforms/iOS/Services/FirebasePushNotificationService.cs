using Firebase.CloudMessaging;
using UserNotifications;

namespace VisitTracker;

public partial class FirebasePushNotificationService
{
    public partial Task<string> GetToken()
    {
        return System.Threading.Tasks.Task.FromResult(Messaging.SharedInstance.FcmToken);
    }

    public partial void ClearAll()
    {
        UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
        UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
    }
}