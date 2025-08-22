namespace VisitTracker.Domain;

/// <summary>
/// Enumeration representing different sensor logc - possible exit reasons.
/// </summary>
public enum ESensorExitReason
{
    UPLOAD_NORMAL_EXIT = 0,
    UPLOAD_OUTSIDE_EXIT = 1,
    UPLOAD_NORMAL_TIMEOUT = 2,
    NO_UPLOAD_ACK_TIMEOUT = 3,
    NO_UPLOAD_SV_TIMEOUT = 4,
    NO_UPLOAD_ACK_TIMEOUT_RESUME = 5,
    NO_UPLOAD_SV_TIMEOUT_RESUME = 6,
    NEXT_VISIT_START = 7,
    HOME_INSIDE_GEOFENCE = 8,
    TIME_TAMPERED = 9,
    VISIT_TERMINATED_OFFLINE = 10,
    UPLOAD_NORMAL_TIMEOUT_RESUME = 11,
    BOOKING_CANCELLED = 12,
}