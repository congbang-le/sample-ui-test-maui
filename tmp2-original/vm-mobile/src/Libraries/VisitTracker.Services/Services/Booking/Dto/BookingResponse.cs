namespace VisitTracker.Services;

public class BookingsResponse
{
    public List<Code> Codes { get; set; }
    public List<VisitMessage> VisitMessages { get; set; }
    public List<CareWorker> CareWorkers { get; set; }
    public List<ServiceUser> ServiceUsers { get; set; }
    public List<ServiceUserAddress> ServiceUsersAddresses { get; set; }
    public List<LocationCentroid> Centroids { get; set; }
    public List<Booking> Bookings { get; set; }
    public List<BookingDetail> BookingDetails { get; set; }
    public List<BookingTask> Tasks { get; set; }
    public List<BookingMedication> Medications { get; set; }
    public List<Visit> Visits { get; set; }
    public List<VisitTask> VisitTasks { get; set; }
    public List<VisitMedication> VisitMedications { get; set; }
    public List<VisitFluid> Fluids { get; set; }
    public List<Incident> Incidents { get; set; }
    public List<BodyMap> BodyMaps { get; set; }
    public List<Attachment> Attachments { get; set; }
    public List<VisitConsumable> Consumables { get; set; }
    public List<VisitShortRemark> ShortRemarks { get; set; }
    public List<VisitHealthStatus> HealthStatuses { get; set; }
}