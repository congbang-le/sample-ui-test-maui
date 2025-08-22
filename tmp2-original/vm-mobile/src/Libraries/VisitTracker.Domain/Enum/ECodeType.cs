namespace VisitTracker.Domain;

/// <summary>
/// Enumeration representing different code types to refer Code table.
/// This enum is used to categorize codes based on their types.
/// </summary>
public enum ECodeType
{
    BOOKING_STATUS,
    VISIT_STATUS,
    TASK_COMPLETION,
    FLUID_COMPLETION,
    MEDICATION_COMPLETION,
    INCIDENT_TYPE,
    INJURY,
    TREATMENT,
    OTHER,
    LOCATION_TYPE,
    MEDICATION_COMPLETED,
    MEDICATION_PARTIAL_COMPLETED,
    MEDICATION_UNABLE_TO_COMPLETE,
    CONSUMABLE_TYPE,
}