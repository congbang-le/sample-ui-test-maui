namespace VisitTracker;

/// <summary>
/// FirebasePushNotificationService is a partial class that provides methods for managing Firebase push notifications.
/// It includes methods for getting the device token and clearing all notifications.
/// </summary>
public partial class FirebasePushNotificationService
{
    /// <summary>
    /// Gets the device token for Firebase push notifications.
    /// This token is used to identify the device for sending push notifications.
    /// </summary>
    /// <returns></returns>
    public partial Task<string> GetToken();

    /// <summary>
    /// Clears all notifications from the notification center.
    /// </summary>
    public partial void ClearAll();
}