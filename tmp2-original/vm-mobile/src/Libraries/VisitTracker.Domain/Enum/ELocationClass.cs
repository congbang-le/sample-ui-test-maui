namespace VisitTracker.Domain;

/// <summary>
/// Enumeration representing different location classes.
/// This enum is used to categorize locations based on their class type.
/// /// The values are as follows:
/// /// G - Ground Truth
/// /// A - Class A (GPS Signals)
/// /// B - Class B (Network Signals)
/// </summary>
public enum ELocationClass
{
    G = 0,
    A = 1,
    B = 2
}