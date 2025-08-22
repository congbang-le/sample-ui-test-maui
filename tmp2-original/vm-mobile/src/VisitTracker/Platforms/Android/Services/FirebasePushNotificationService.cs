using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Firebase.Messaging;

namespace VisitTracker;

public partial class FirebasePushNotificationService
{
    public async partial Task<string> GetToken()
    {
        var token = await FirebaseMessaging.Instance.GetToken().AsAsync<Java.Lang.String>();
        return token.ToString();
    }

    public partial void ClearAll()
    {
        NotificationManager notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);
        notificationManager.CancelAll();
    }
}