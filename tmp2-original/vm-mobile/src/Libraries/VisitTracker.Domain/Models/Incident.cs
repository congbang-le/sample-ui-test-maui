namespace VisitTracker.Domain;

public class Incident : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    [JsonPropertyName("Id")]
    public int ServerRef { get; set; }

    public string Summary { get; set; }
    public bool DuringOnGoingVisit { get; set; }

    public bool AtSuLocation { get; set; }
    public string Location { get; set; }
    public string Geolocation { get; set; }

    public int TypeId { get; set; }
    public int InjuryId { get; set; }
    public int TreatmentId { get; set; }
    public string OtherInjury { get; set; }
    public string OtherIncidentType { get; set; }

    public string SubmittedByName { get; set; }
    public string SubmittedByType { get; set; }
    public int SubmittedById { get; set; }

    public int? VisitId { get; set; }
    public int ServiceUserId { get; set; }

    [JsonIgnore]
    public long IncidentDateTimeTicks { get; set; }

    [Ignored]
    public DateTime IncidentDateTime
    {
        get { return new DateTime(IncidentDateTimeTicks); }
        set { IncidentDateTimeTicks = value.Ticks; }
    }

    [JsonIgnore]
    public long SubmittedOnTicks { get; set; }

    [Ignored]
    public DateTime SubmittedOn
    {
        get { return new DateTime(SubmittedOnTicks); }
        set { SubmittedOnTicks = value.Ticks; }
    }

    [JsonIgnore]
    public long CompletedOnTicks { get; set; }

    [Ignored]
    public DateTime? CompletedOn
    {
        get { return CompletedOnTicks != 0 ? new DateTime(CompletedOnTicks) : null; }
        set { CompletedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    public bool IsSaved { get; set; } = false;
    public bool UnSaved { get; set; } = false;

    [NotMapped]
    public string DisplayName { get; set; }
}