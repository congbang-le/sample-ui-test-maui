namespace VisitTracker;

/// <summary>
/// EFeaturePermissionOrderedType is an enumeration that defines the different types of feature permissions that can be requested in the application.
/// </summary>
public enum EFeaturePermissionOrderedType
{
    Location = 1000,
    Activity = 1001,
    Battery = 1002,
    Notification = 1003,
    Internet = 1004,
    ClockTampered = 1005
}