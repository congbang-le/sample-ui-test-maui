namespace VisitTracker.Domain;

public class ServiceUserAddress : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public int ServiceUserId { get; set; }

    public string Address => string.Join(" , ", [Address1, PostCode]);
    public string Address1 { get; set; }
    public string PostCode { get; set; }

    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string Location { get; set; }

    public string KeySafePIN { get; set; }
    public string EntryInstructions { get; set; }
    public string HousePhotos { get; set; }

    [JsonIgnore]
    public long EffectiveFromTicks { get; set; }

    [Ignored]
    public DateTime EffectiveFrom
    {
        get { return new DateTime(EffectiveFromTicks); }
        set { EffectiveFromTicks = value.Ticks; }
    }

    [JsonIgnore]
    public long EffectiveToTicks { get; set; }

    [Ignored]
    public DateTime? EffectiveTo
    {
        get { return EffectiveToTicks != 0 ? new DateTime(EffectiveToTicks) : null; }
        set { EffectiveToTicks = value.HasValue ? value.Value.Ticks : default; }
    }
}