namespace VisitTracker.Services;

public class VisitReportEditDto
{
    public int BookingDetailId { get; set; }
    public bool DoPostProcessing { get; set; }
    public Visit Visit { get; set; }
    public VisitFluid VisitFluid { get; set; }
    public IList<VisitTask> VisitTaskList { get; set; }
    public IList<VisitMedication> VisitMedicationList { get; set; }
    public IList<Incident> IncidentList { get; set; }
    public IList<BodyMap> BodyMapList { get; set; }
    public IList<Attachment> AttachmentList { get; set; }
    public IList<VisitConsumable> ConsumableList { get; set; }
    public IList<VisitShortRemark> ShortRemarkList { get; set; }
    public IList<VisitHealthStatus> HealthStatusList { get; set; }
}