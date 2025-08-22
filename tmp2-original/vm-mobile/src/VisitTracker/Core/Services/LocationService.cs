namespace VisitTracker;

/// <summary>
/// CareWorkerLocationService is a platform specific partial class that provides methods for managing sensing logic in the application for the Care Workers.
/// </summary>
public partial class CareWorkerLocationService
{
    /// <summary>
    /// Starts the Normal mode of the Sensing logic for the Care Worker.
    /// </summary>
    public partial void StartNormalMode();

    /// <summary>
    /// Stops the Normal mode of the Sensing logic for the Care Worker.
    /// </summary>
    public partial void StopNormalMode();

    /// <summary>
    /// Starts the Tracking mode of the Sensing logic for the Care Worker starting with the location provided.
    /// </summary>
    /// <param name="lat">latitude</param>
    /// <param name="lon">longitude</param>
    public partial void StartTrackingMode(double lat, double lon);

    /// <summary>
    /// Stops the Tracking mode of the Sensing logic for the Care Worker.
    /// </summary>
    private partial void StopTrackingMode();
}

/// <summary>
/// SupervisorLocationService is a platform specific partial class that provides methods for managing sensing logic in the application for the Supervisors.
/// </summary>
public partial class SupervisorLocationService
{
    /// <summary>
    /// Starts the Fingerprint logic for the Supervisor.
    /// </summary>
    public partial void StartFingerprint();

    /// <summary>
    /// Stops the Fingerprint logic for the Supervisor.
    /// </summary>
    public partial void StopFingerprint();

    /// <summary>
    /// Starts the Normal mode of the Sensing logic for the Supervisor.
    /// </summary>
    public partial void StartNormalMode();

    /// <summary>
    /// Stops the Normal mode of the Sensing logic for the Supervisor.
    /// </summary>
    public partial void StopNormalMode();

    /// <summary>
    /// Starts the Tracking mode of the Sensing logic for the Supervisor starting with the location provided.
    /// </summary>
    /// <param name="lat">latitude</param>
    /// <param name="lon">longitude</param>
    public partial void StartTrackingMode(double lat, double lon);

    /// <summary>
    /// Stops the Tracking mode of the Sensing logic for the Supervisor.
    /// </summary>
    private partial void StopTrackingMode();
}