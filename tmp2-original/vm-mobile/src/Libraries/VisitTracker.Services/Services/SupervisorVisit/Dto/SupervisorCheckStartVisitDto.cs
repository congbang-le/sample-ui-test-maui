namespace VisitTracker.Services;

public class SupervisorCheckStartVisitDto
{
    public int SupervisorId { get; set; }
    public int ServiceUserId { get; set; }
    public string DeviceInfo { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}