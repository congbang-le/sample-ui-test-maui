namespace VisitTracker.Services;

public class IncidentAdhocRequest
{
    public Incident Incident { get; set; }
    public List<BodyMap> BodyMaps { get; set; }
    public List<Attachment> Attachments { get; set; }
}