using Foundation;
using Security;

namespace VisitTracker;

public partial class BiometricSnapshotService
{
    private const string iOSAccountName = "iOSKeyStore";

    public partial bool SnapshotBiometricState()
    {
        var accessControl = new SecAccessControl(SecAccessible.WhenPasscodeSetThisDeviceOnly,
            SecAccessControlCreateFlags.BiometryCurrentSet);

        var keyAttributes = new SecRecord(SecKind.GenericPassword)
        {
            Service = BiometricKeyName,
            Account = iOSAccountName,
            AccessControl = accessControl,
            ValueData = NSData.FromString("SecureData")
        };

        var status = SecKeyChain.Add(keyAttributes);
        return status == SecStatusCode.Success;
    }

    public partial void ClearBiometricState()
    {
        var query = new SecRecord(SecKind.GenericPassword)
        {
            Service = BiometricKeyName,
            Account = iOSAccountName
        };
        SecKeyChain.Remove(query);
    }

    public partial bool? HasBiometricStateChanged()
    {
        if (!AppServices.Current.AppPreference.HasBiometricReg)
            return null;

        var query = new SecRecord(SecKind.GenericPassword)
        {
            Service = BiometricKeyName,
            Account = iOSAccountName
        };

        var result = SecKeyChain.QueryAsRecord(query, out var status);
        return status == SecStatusCode.ItemNotFound;
    }
}