namespace VisitTracker.Services;

/// <summary>
/// Interface for managing application preferences and settings.
/// This interface provides properties and methods to access and manipulate user preferences.
/// </summary>
public interface IAppPreference
{
    string PrefKeyDeviceId { get; }
    string DeviceID { get; }

    string PushToken { get; set; }

    int UserId { get; set; }
    string User { get; set; }

    string DeviceInfo { get; set; }
    public bool HasBiometricReg { get; set; }

    void Clear();
}