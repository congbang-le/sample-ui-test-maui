using CoreLocation;
using CoreMotion;
using Foundation;
using UIKit;
using UserNotifications;

namespace VisitTracker;

public partial class FeaturePermissionHandlerService
{
    public partial bool CheckPermission(EFeaturePermissionOrderedType type)
    {
        if (type == EFeaturePermissionOrderedType.Location)
            return new CLLocationManager().AuthorizationStatus == CLAuthorizationStatus.AuthorizedAlways;
        else if (type == EFeaturePermissionOrderedType.Activity)
            return CMPedometer.AuthorizationStatus == CMAuthorizationStatus.Authorized;
        else return true;
    }

    public partial void RequestPermission(EFeaturePermissionOrderedType type)
    {
        if (type == EFeaturePermissionOrderedType.Location)
        {
            var locationManager = new CLLocationManager();
            locationManager.RequestAlwaysAuthorization();
            if (locationManager.AuthorizationStatus == CLAuthorizationStatus.Denied)
                OpenSettings(EFeaturePermissionOrderedType.Location);
        }
        else if (type == EFeaturePermissionOrderedType.Activity)
        {
            var pedometer = new CMPedometer();
            pedometer.QueryPedometerData(NSDate.Now, NSDate.DistantFuture, (data, error) =>
            {
                if (error != null)
                    OpenSettings(EFeaturePermissionOrderedType.Activity);
            });
        }
        else if (type == EFeaturePermissionOrderedType.Notification)
            MainThread.BeginInvokeOnMainThread(async () => await UNUserNotificationCenter.Current.RequestAuthorizationAsync(UNAuthorizationOptions.CriticalAlert));
    }

    public partial async Task<bool> CheckEnabled(EFeaturePermissionOrderedType type)
    {
        if (type == EFeaturePermissionOrderedType.Location)
            return CLLocationManager.LocationServicesEnabled;
        else if (type == EFeaturePermissionOrderedType.Internet)
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        else if (type == EFeaturePermissionOrderedType.ClockTampered)
            return !await AppServices.Current.TamperingService.IsTimeTampered();
        else if (type == EFeaturePermissionOrderedType.Notification)
        {
            var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
            return settings.AuthorizationStatus == UNAuthorizationStatus.Authorized;
        }
        else return true;
    }

    public partial void RequestEnabled(EFeaturePermissionOrderedType type)
    {
        if (type == EFeaturePermissionOrderedType.Location)
            OpenSettings(EFeaturePermissionOrderedType.Location);
        else if (type == EFeaturePermissionOrderedType.Internet)
            OpenSettings(EFeaturePermissionOrderedType.Internet);
        else if (type == EFeaturePermissionOrderedType.ClockTampered)
            OpenSettings(EFeaturePermissionOrderedType.ClockTampered);
        else if (type == EFeaturePermissionOrderedType.Notification)
            OpenSettings(EFeaturePermissionOrderedType.Notification);
    }

    public partial void OpenSettingsPage()
    {
        OpenSettings();
    }

    private void OpenSettings(EFeaturePermissionOrderedType? type = null)
    {
        NSUrl settingsUrl = type switch
        {
            EFeaturePermissionOrderedType.Notification => new NSUrl(UIApplication.OpenNotificationSettingsUrl),
            _ => new NSUrl(UIApplication.OpenSettingsUrlString),
        };

        MainThread.BeginInvokeOnMainThread(() =>
        {
            UIApplication.SharedApplication.OpenUrl(settingsUrl);
        });
    }
}