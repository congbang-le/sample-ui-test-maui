using Android.App;
using Android.Content;
using Android.OS;
using Firebase.Messaging;
using Java.Lang;

namespace VisitTracker;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
public class FirebaseMessagingService : Firebase.Messaging.FirebaseMessagingService
{
    public override async void OnMessageReceived(RemoteMessage message)
    {
        string PushNotificationChannelId = "channel_push_notify";

        message.Data.TryGetValue("action", out var notificationDict);

        var pushNotificationData = JsonExtensions.Deserialize<Domain.Notification>(notificationDict);
        if (pushNotificationData != null && !string.IsNullOrEmpty(pushNotificationData.Title)
                && !string.IsNullOrEmpty(pushNotificationData.Message))
        {
            if (!NotificationHelper.Current.NonPersistentNotificationTypes.Contains(
                    (ENotificationType)System.Enum.Parse(typeof(ENotificationType), pushNotificationData.NotificationType)))
                pushNotificationData = await AppServices.Current.NotificationService.InsertOrReplace(pushNotificationData);

            if (!pushNotificationData.SuppressNotification)
            {
                Random random = new Random();

                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra(nameof(Domain.Notification), JsonExtensions.Serialize(pushNotificationData));
                intent.AddFlags(ActivityFlags.SingleTop);
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, random.Next(99999), intent, PendingIntentFlags.Immutable);

                var builder = new Android.App.Notification.Builder(ApplicationContext, PushNotificationChannelId)
                                .SetContentIntent(pendingIntent)
                                .SetContentTitle(pushNotificationData.Title)
                                .SetContentText(pushNotificationData.Message)
                                .SetAutoCancel(true)
                                .SetSmallIcon(Resource.Mipmap.appicon)
                                .SetWhen(JavaSystem.CurrentTimeMillis());

                NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    NotificationChannel channel = new NotificationChannel(PushNotificationChannelId, "Push Notification Channel", NotificationImportance.High);
                    manager.CreateNotificationChannel(channel);
                }

                Android.App.Notification notification = builder.Build();

                if (NotificationHelper.Current.NonDuplicateNotificationTypes.Contains(
                        (ENotificationType)System.Enum.Parse(typeof(ENotificationType), pushNotificationData.NotificationType)))
                    manager.Cancel(pushNotificationData.NotificationTypeId);

                manager.Notify(pushNotificationData.NotificationTypeId, notification);
            }

            if (AppServices.Current.AppPreference.UserId != default)
                await NotificationHelper.Current.OnNotificationReceived(pushNotificationData);
        }
    }

    public override void OnNewToken(string token)
    {
        base.OnNewToken(token);

        AppServices.Current.AppPreference.PushToken = token;
        if (AppServices.Current.AppPreference.UserId != default)
            Task.Run(AppServices.Current.AuthService.RegisterDeviceForPushNotifications);
    }
}