namespace VisitTracker;

/// <summary>
/// BiometricSnapshotService is a service that manages the biometric state of the application.
/// It provides methods to take a snapshot of the biometric state, clear the biometric state, and check if the biometric state has changed.
/// </summary>
public partial class BiometricSnapshotService
{
    private readonly string BiometricKeyName = "com.artivim.vm.biometric.key";

    /// <summary>
    /// Takes a snapshot of the biometric state and returns a boolean indicating whether the snapshot was successful or not.
    /// This method is used to capture the current biometric state of the application, which can be used for comparison later.
    /// </summary>
    /// <returns></returns>
    public partial bool SnapshotBiometricState();

    /// <summary>
    /// Clears the biometric state of the application.
    /// </summary>
    public partial void ClearBiometricState();

    /// <summary>
    /// Checks if the biometric state has changed since the last snapshot.
    /// This method compares the current biometric state with the previously captured snapshot and returns a boolean indicating whether there has been a change or not.
    /// </summary>
    /// <returns></returns>
    public partial bool? HasBiometricStateChanged();
}