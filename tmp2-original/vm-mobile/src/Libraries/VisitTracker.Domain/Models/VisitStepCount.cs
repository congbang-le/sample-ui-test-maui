namespace VisitTracker.Domain;

public class VisitStepCount : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    public int? VisitId { get; set; }
    public int? SupervisorVisitId { get; set; }

    public int CountTillNow { get; set; }

    public float DistanceTillNow { get; set; }

    public float AzimuthNow { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    [JsonIgnore]
    public long OnTimeTicks { get; set; }

    [Ignored]
    public DateTime OnTime
    {
        get { return new DateTime(OnTimeTicks); }
        set { OnTimeTicks = value.Ticks; }
    }
}