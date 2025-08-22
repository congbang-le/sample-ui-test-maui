namespace VisitTracker.Services;

public class LastKnownDto
{
    public int CareWorkerId { get; set; }
    public int BookingDetailId { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public DateTime LocOnTime { get; set; }
    public string DeviceInfo { get; set; }
}