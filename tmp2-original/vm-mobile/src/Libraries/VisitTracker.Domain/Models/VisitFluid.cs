namespace VisitTracker.Domain;

public class VisitFluid : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    [JsonPropertyName("Id")]
    public int ServerRef { get; set; }

    public int VisitId { get; set; }
    public int ServiceUserId { get; set; }

    public int? OralIntake { get; set; }
    public int? IvScIntake { get; set; }
    public int? OtherIntake { get; set; }
    public int? UrineOutput { get; set; }
    public int? VomitOutput { get; set; }
    public int? TubeOutput { get; set; }
    public int? OtherOutput { get; set; }

    public string Summary { get; set; }

    [JsonIgnore]
    public long CompletedOnTicks { get; set; }

    [Ignored]
    public DateTime? CompletedOn
    {
        get { return CompletedOnTicks != 0 ? new DateTime(CompletedOnTicks) : null; }
        set { CompletedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    [Ignored] public string Hour { get; set; }

    public bool IsSaved { get; set; }
}