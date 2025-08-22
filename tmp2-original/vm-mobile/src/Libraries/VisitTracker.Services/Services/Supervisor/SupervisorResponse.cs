namespace VisitTracker.Services;

public class SupervisorFormsResponse
{
    public int PendingFormsCount { get; set; }
    public int ScheduledFormsCount { get; set; }
    public int SubmittedFormsCount { get; set; }
    public int OverdueFormsCount { get; set; }
}