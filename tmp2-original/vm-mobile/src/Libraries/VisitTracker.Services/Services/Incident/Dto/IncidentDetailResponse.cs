namespace VisitTracker.Services;

public class IncidentDetailResponse
{
    public Incident Incident { get; set; }
    public int ServiceUserId { get; set; }
    public Booking Booking { get; set; }
    public IList<BodyMap> BodyMaps { get; set; }
    public IList<Attachment> Attachments { get; set; }
}