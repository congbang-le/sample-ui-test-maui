using Android;

namespace VisitTracker;

public class AllPermissionsHelper : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            (Manifest.Permission.ActivityRecognition, true),
            (Manifest.Permission.AccessFineLocation, true),
            (Manifest.Permission.PostNotifications, true),
            (Manifest.Permission.RequestIgnoreBatteryOptimizations, true)
        }.ToArray();
}

public class ActivityPermissionHelper : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            (Manifest.Permission.ActivityRecognition, true)
        }.ToArray();
}

public class NotificationPermissionHelper : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            (Manifest.Permission.PostNotifications, true)
        }.ToArray();
}

public class LocationBackgroundPermissionsHelper : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            (Manifest.Permission.AccessBackgroundLocation, true)
        }.ToArray();
}