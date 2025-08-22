namespace VisitTracker.Services;

public class AppPreference : IAppPreference
{
    public string PrefKeyDeviceId { get; } = "PrefKey-DeviceID";
    private const string PrefKeyPushToken = "PrefKey-PushToken";
    private const string PrefKeyUserId = "PrefKey-UserId";
    private const string PrefKeyUser = "PrefKey-User";
    private const string PrefKeyDeviceInfo = "PrefKey-DeviceInfo";
    private const string PrefKeyHasBiometricReg = "PrefKey-HasBiometricReg";

    public string DeviceID
    {
        get
        {
            if (!Preferences.Default.ContainsKey(PrefKeyDeviceId))
                Preferences.Default.Set(PrefKeyDeviceId, Guid.NewGuid().ToString());

            return Preferences.Default.Get(PrefKeyDeviceId, string.Empty);
        }
    }

    public string PushToken
    {
        get => Preferences.Default.Get(PrefKeyPushToken, string.Empty);
        set
        {
            if (!string.IsNullOrEmpty(value))
                Preferences.Default.Set(PrefKeyPushToken, value);
        }
    }

    public int UserId
    {
        get => Preferences.Default.Get(PrefKeyUserId, default(int));
        set
        {
            Preferences.Default.Set(PrefKeyUserId, value);
        }
    }

    public string User
    {
        get => Preferences.Default.Get(PrefKeyUser, string.Empty);
        set
        {
            Preferences.Default.Set(PrefKeyUser, value);
        }
    }

    public string DeviceInfo
    {
        get => Preferences.Default.Get(PrefKeyDeviceInfo, string.Empty);
        set
        {
            if (!string.IsNullOrEmpty(value))
                Preferences.Default.Set(PrefKeyDeviceInfo, value);
        }
    }

    public bool HasBiometricReg
    {
        get => Preferences.Default.Get(PrefKeyHasBiometricReg, false);
        set
        {
            Preferences.Default.Set(PrefKeyHasBiometricReg, value);
        }
    }

    public void Clear()
    {
        PushToken = string.Empty;
        UserId = default;
        User = string.Empty;
        DeviceInfo = string.Empty;
    }
}