namespace VisitTracker.Domain;

public class BodyMap : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    [JsonPropertyName("Id")]
    public int ServerRef { get; set; }

    public string Parts { get; set; }

    public string Notes { get; set; }

    public int? BaseVisitId { get; set; }
    public int? VisitId { get; set; }
    public int? VisitTaskId { get; set; }
    public int? VisitMedicationId { get; set; }
    public int? VisitFluidId { get; set; }
    public int? IncidentId { get; set; }

    [JsonIgnore]
    public long AddedOnTicks { get; set; }

    [Ignored]
    public DateTime AddedOn
    {
        get { return new DateTime(AddedOnTicks); }
        set { AddedOnTicks = value.Ticks; }
    }

    [JsonIgnore] public bool IsSaved { get; set; } = false;
    [JsonIgnore] public bool UnSaved { get; set; } = false;
}