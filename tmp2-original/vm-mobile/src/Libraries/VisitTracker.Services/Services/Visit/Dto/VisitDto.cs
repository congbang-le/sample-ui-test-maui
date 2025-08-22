namespace VisitTracker.Services;

public class VisitDto
{
    public IList<Visit> Visits { get; set; }
    public IList<VisitTask> VisitTasks { get; set; }
    public IList<VisitMedication> VisitMedications { get; set; }
    public IList<VisitFluid> Fluids { get; set; }
    public IList<Incident> Incidents { get; set; }
    public IList<BodyMap> BodyMaps { get; set; }
    public IList<Attachment> Attachments { get; set; }
    public IList<VisitConsumable> Consumables { get; set; }
    public IList<VisitShortRemark> ShortRemarks { get; set; }
    public IList<VisitHealthStatus> HealthStatuses { get; set; }
}