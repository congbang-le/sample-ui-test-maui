namespace VisitTracker.Domain;

public class VisitMedication : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    [JsonPropertyName("Id")]
    public int ServerRef { get; set; }

    public int VisitId { get; set; }
    public int MedicationId { get; set; }

    public string Summary { get; set; }
    public int? CompletionStatusId { get; set; }
    public int? CompletionStatusDetailId { get; set; }

    public bool HasSentLateMedRequest { get; set; }
    public bool MedLowSupply { get; set; }
    public int? MedLowSupplyQuantity { get; set; }

    [JsonIgnore]
    public long CompletedOnTicks { get; set; }

    [Ignored]
    public DateTime? CompletedOn
    {
        get { return CompletedOnTicks != 0 ? new DateTime(CompletedOnTicks) : null; }
        set { CompletedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    [JsonIgnore]
    public long StartDateTimeTicks { get; set; }

    [Ignored]
    public DateTime StartDateTime
    {
        get { return new DateTime(StartDateTimeTicks); }
        set { StartDateTimeTicks = value.Ticks; }
    }

    [JsonIgnore]
    public long EndDateTimeTicks { get; set; }

    [Ignored]
    public DateTime EndDateTime
    {
        get { return new DateTime(EndDateTimeTicks); }
        set { EndDateTimeTicks = value.Ticks; }
    }

    public bool? CanAdmisterMedication { get; set; }
    public bool HasNoResponse { get; set; } = false;
    public bool IsSaved { get; set; } = false;
}