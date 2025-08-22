namespace VisitTracker.Domain;

public class VisitTask : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    [JsonPropertyName("Id")]
    public int ServerRef { get; set; }

    public string Summary { get; set; }

    public int VisitId { get; set; }
    public int TaskId { get; set; }
    public int? CompletionStatusId { get; set; }

    [JsonIgnore]
    public long CompletedOnTicks { get; set; }

    [Ignored]
    public DateTime? CompletedOn
    {
        get { return CompletedOnTicks != 0 ? new DateTime(CompletedOnTicks) : null; }
        set { CompletedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    [JsonIgnore]
    public bool IsSaved { get; set; }
}