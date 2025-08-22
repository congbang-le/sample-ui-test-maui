namespace VisitTracker.Services;

public class VisitReportSensorDto
{
    public int BookingDetailId { get; set; }
    public bool DoPostProcessing { get; set; }
    public Visit Visit { get; set; }
    public IList<VisitLocation> LocationList { get; set; }
    public IList<VisitStepCount> StepCountList { get; set; }
}