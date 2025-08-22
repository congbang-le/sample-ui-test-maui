namespace VisitTracker.Services;

public class SupervisorVisitReportSensorDto
{
    public SupervisorVisit SupervisorVisit { get; set; }
    public bool DoPostProcessing { get; set; }

    public List<VisitLocation> LocationList { get; set; }
    public List<VisitStepCount> StepCountList { get; set; }
}