using Android;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Provider;
using AndroidX.Core.App;
using AndroidApp = Android.App.Application;

namespace VisitTracker;

public partial class FeaturePermissionHandlerService
{
    public Activity CurrentActivity => ActivityStateManager.Default.GetCurrentActivity();

    public partial bool CheckPermission(EFeaturePermissionOrderedType type)
    {
        if (type == EFeaturePermissionOrderedType.Location)
            return CheckAndroidPermission(Manifest.Permission.AccessFineLocation);
        else if (type == EFeaturePermissionOrderedType.Activity)
            return CheckAndroidPermission(Manifest.Permission.ActivityRecognition);
        else if (type == EFeaturePermissionOrderedType.Battery)
        {
            PowerManager pm = (PowerManager)AndroidApp.Context.GetSystemService(Context.PowerService);
            return pm.IsIgnoringBatteryOptimizations(AppInfo.PackageName);
        }
        else return true;
    }

    public partial void RequestPermission(EFeaturePermissionOrderedType type)
    {
        if (type == EFeaturePermissionOrderedType.Location)
            RequestAndroidPermission(Manifest.Permission.AccessFineLocation, (int)EFeaturePermissionOrderedType.Location);
        else if (type == EFeaturePermissionOrderedType.Activity)
            RequestAndroidPermission(Manifest.Permission.ActivityRecognition, (int)EFeaturePermissionOrderedType.Activity);
        else if (type == EFeaturePermissionOrderedType.Battery)
        {
            PowerManager pm = (PowerManager)AndroidApp.Context.GetSystemService(Context.PowerService);
            if (!pm.IsIgnoringBatteryOptimizations(AppInfo.PackageName))
                OpenSettingsDetail(Settings.ActionRequestIgnoreBatteryOptimizations);
        }
        else if (type == EFeaturePermissionOrderedType.Notification)
            RequestAndroidPermission(Manifest.Permission.PostNotifications, (int)EFeaturePermissionOrderedType.Notification);
    }

    public async partial Task<bool> CheckEnabled(EFeaturePermissionOrderedType type)
    {
        if (type == EFeaturePermissionOrderedType.Location)
        {
            LocationManager locationManager = (LocationManager)AndroidApp.Context.GetSystemService(Context.LocationService);
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }
        else if (type == EFeaturePermissionOrderedType.Internet)
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        else if (type == EFeaturePermissionOrderedType.ClockTampered)
            return !await AppServices.Current.TamperingService.IsTimeTampered();
        else if (type == EFeaturePermissionOrderedType.Notification)
            return NotificationManagerCompat.From(AndroidApp.Context).AreNotificationsEnabled();
        else return true;
    }

    public partial void RequestEnabled(EFeaturePermissionOrderedType type)
    {
        if (type == EFeaturePermissionOrderedType.Location)
            OpenSettingsDetail(Settings.ActionLocationSourceSettings);
        else if (type == EFeaturePermissionOrderedType.Internet)
            OpenSettingsDetail(Settings.ActionWirelessSettings);
        else if (type == EFeaturePermissionOrderedType.ClockTampered)
            OpenSettingsDetail(Settings.ActionDateSettings);
        else if (type == EFeaturePermissionOrderedType.Notification)
            RequestAndroidPermission(Manifest.Permission.PostNotifications, (int)EFeaturePermissionOrderedType.Notification);
    }

    public partial void OpenSettingsPage()
    {
        OpenSettingsDetail(Settings.ActionApplicationDetailsSettings);
    }

    private bool CheckAndroidPermission(string permission)
    {
        return ActivityCompat.CheckSelfPermission(CurrentActivity, permission) == Android.Content.PM.Permission.Granted;
    }

    private void RequestAndroidPermission(string permission, int permissionCode)
    {
        if (!ActivityCompat.ShouldShowRequestPermissionRationale(CurrentActivity, permission))
            ActivityCompat.RequestPermissions(CurrentActivity, new[] { permission }, permissionCode);
        else OpenSettingsDetail(Settings.ActionApplicationDetailsSettings);
    }

    private void OpenSettingsDetail(string settingsDetail)
    {
        var settingsIntent = new Intent();
        settingsIntent.SetAction(settingsDetail);
        settingsIntent.AddCategory(Intent.CategoryDefault);
        if (settingsDetail == Settings.ActionApplicationDetailsSettings
                || settingsDetail == Settings.ActionRequestIgnoreBatteryOptimizations)
            settingsIntent.SetData(Android.Net.Uri.Parse("package:" + AppInfo.PackageName));

        var flags = ActivityFlags.NewTask | ActivityFlags.NoHistory | ActivityFlags.ExcludeFromRecents;
        settingsIntent.SetFlags(flags);

        CurrentActivity.StartActivity(settingsIntent);
    }
}