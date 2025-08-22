namespace VisitTracker.Services;

public class SupervisorStartVisitResponseDto
{
    public bool IsInsideGeozone { get; set; }
    public bool AnyPendingVisits { get; set; }
}