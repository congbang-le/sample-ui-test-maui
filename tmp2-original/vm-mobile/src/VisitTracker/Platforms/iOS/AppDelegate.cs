using Firebase.CloudMessaging;
using Foundation;
using UIKit;
using UserNotifications;

namespace VisitTracker;

[Register(nameof(AppDelegate))]
public class AppDelegate : MauiUIApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        try
        {
            Firebase.Core.App.Configure();
            RegisterFirebaseNotifications();
        }
        catch (Exception ex)
        {
            throw;
        }

        return base.FinishedLaunching(app, options);
    }

    private void RegisterFirebaseNotifications()
    {
        if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
        {
            var authOptions = UNAuthorizationOptions.Alert
                             | UNAuthorizationOptions.Badge
                             | UNAuthorizationOptions.Sound;

            UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
            {
                Console.WriteLine(granted);
            });

            // For iOS 10 display notification (sent via APNS)
            UNUserNotificationCenter.Current.Delegate = this;

            // For iOS 10 data message (sent via FCM)
            Messaging.SharedInstance.Delegate = this as IMessagingDelegate;
        }

        UIApplication.SharedApplication.RegisterForRemoteNotifications();
    }

    [Export("messaging:didReceiveRegistrationToken:")]
    public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
    {
        AppServices.Current.AppPreference.PushToken = fcmToken;

        if (AppServices.Current.AppPreference.UserId != default)
            Task.Run(AppServices.Current.AuthService.RegisterDeviceForPushNotifications);
    }

    /// <summary>
    /// Called for when the app is in the foreground or background and a remote notification is received.
    /// </summary>
    [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
    public void DidReceiveRemoteNotification(UIApplication application, NSDictionary data, Action<UIBackgroundFetchResult> completionHandler)
    {
        HandleNotification(data,false);
    }

    /// <summary>
    /// Called when the user  clicked at presented notification while the app was in the foreground or background.
    /// </summary>
    [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
    public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
    {
        HandleNotification(response.Notification.Request.Content.UserInfo, true);
        completionHandler();
    }

    /// <summary>
    /// Called when a notification is received while the app is in the foreground.
    /// handles the notificaton without opening it.
    /// </summary>
    [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
    public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        NSDictionary data = notification.Request.Content.UserInfo;
        HandleNotification(data,false);
        completionHandler(UNNotificationPresentationOptions.Alert);
    }

    [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
    public void RegisteredForRemoteNotifications(UIKit.UIApplication application, NSData deviceToken)
    {
        Messaging.SharedInstance.ApnsToken = deviceToken;
    }

    [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
    public void FailedToRegisterForRemoteNotifications(UIKit.UIApplication application, NSError error)
    { }

    /// <summary>
    /// Handles the notification received from Firebase.
    /// This method processes the notification data, deserializes it into a Domain.Notification object,
    /// </summary>
    private void HandleNotification(NSDictionary data, bool open)
    {
        if (data != null)
        {
            if (!(data["action"] is NSString actionString))
                throw new InvalidOperationException("'action' key is missing or not a string in push notification payload.");

            Task.Run(async () =>
            {
                var dbNotification = JsonExtensions.Deserialize<Domain.Notification>(actionString);
                if (dbNotification != null && !string.IsNullOrEmpty(dbNotification.Title)
                    && !string.IsNullOrEmpty(dbNotification.Message))
                {
                    if (!NotificationHelper.Current.NonPersistentNotificationTypes.Contains(
                            (ENotificationType)System.Enum.Parse(typeof(ENotificationType), dbNotification.NotificationType)))
                        dbNotification = await AppServices.Current.NotificationService.InsertOrReplace(dbNotification);

                    if (AppServices.Current.AppPreference.UserId != default)
                        await NotificationHelper.Current.OnNotificationReceived(dbNotification);

                    if (open) WeakReferenceMessenger.Default.Send(new MessagingEvents.NotificationMessage(dbNotification.Id));
                }
            });
        }
    }
}