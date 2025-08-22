namespace VisitTracker.Services;

/// <summary>
/// Service for detecting time tampering on the device.
/// This service compares the device's current time with the server's time to check for discrepancies.
/// </summary>
public class TamperingService
{
    private readonly IAuthService _authService;

    public TamperingService(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Checks if the device's time is tampered by comparing it with the server's time.
    /// If the difference exceeds a predefined tolerance, it indicates potential tampering.
    /// </summary>
    /// <returns>true if tampered, false if not tampered</returns>
    public async Task<bool> IsTimeTampered()
    {
        var internetTime = await _authService.GetServerTime();
        var deviceTime = DateTimeExtensions.NowNoTimezone();

        // Check if the device time falls outside the tolerance window
        var difference = (int)Math.Abs((deviceTime - internetTime.Value).TotalSeconds);

        return difference > Constants.TimeTamperingToleranceInSecs;
    }
}