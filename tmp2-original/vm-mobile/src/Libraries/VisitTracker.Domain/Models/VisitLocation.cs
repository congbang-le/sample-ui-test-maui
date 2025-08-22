namespace VisitTracker.Domain;

public class VisitLocation : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    public int? SupervisorVisitId { get; set; }
    public int? VisitId { get; set; }
    public int? ServiceUserFpId { get; set; }

    [Ignored] public string Coordinate => Latitude + "," + Longitude;

    [Ignored] public bool HasEnteredGeozone { get; set; }

    [JsonIgnore] public bool IsSync { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public double LocationAccuracy { get; set; }

    public double Altitude { get; set; }

    public double AltitudeAccuracy { get; set; }

    public double Speed { get; set; }

    public double Heading { get; set; }

    public double HeadingAccuracy { get; set; }

    public string Provider { get; set; }

    public string LocationClass { get; set; }

    public int Platform { get; set; }

    [JsonIgnore]
    public long OnTimeTicks { get; set; }

    [Ignored]
    public DateTime OnTime
    {
        get { return new DateTime(OnTimeTicks); }
        set { OnTimeTicks = value.Ticks; }
    }
}