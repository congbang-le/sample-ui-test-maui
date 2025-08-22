namespace VisitTracker.Domain;

/// <summary>
/// Enumeration representing different code names to refer Code table.
/// This enum is used to categorize codes based on their name.
/// </summary>
public enum ECodeName
{
    SCHEDULED,
    PROGRESS,
    RESCHEDULED,
    CANCELLED,
    MISSED,
    UNABLE_TO_SUBMIT,
    COMPLETED,
    TAMPERED,
    ACKNOWLEDGED,
    STARTED,
    INVALIDATED,

    BY_CAREWORKER,
    BY_SERVICE_USER,
    BY_NEXT_OF_KIN,
    BY_OTHERS,
    UNABLE_TO_COMPLETE,
    PARTIALLY_COMPLETED,
    INSUFFICIENT_DOSE,
    SERVICE_USER_REFUSED,
    DOSE_MISSING,
    MISSED_VISIT,
    NOT_APPROVED
}