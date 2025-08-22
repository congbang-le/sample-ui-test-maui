namespace VisitTracker.Domain;

public class BookingDetail : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public int BookingId { get; set; }
    public int CareWorkerId { get; set; }
    public int TransportationId { get; set; }

    public int TravelTime { get; set; }
    public bool IsMaster { get; set; }

    public string Eta { get; set; }
    public string EtaOn { get; set; }
    public string EtaStatusColor { get; set; }
    public string EtaStatusText { get; set; }
    public bool EtaAvailable { get; set; }
}