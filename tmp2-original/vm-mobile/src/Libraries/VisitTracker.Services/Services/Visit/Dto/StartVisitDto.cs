namespace VisitTracker.Services;

public class StartVisitDto
{
    public int BookingDetailId { get; set; }
    public string DeviceInfo { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}