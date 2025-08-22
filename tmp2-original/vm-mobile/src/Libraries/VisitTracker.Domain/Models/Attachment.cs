namespace VisitTracker.Domain;

public class Attachment : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    [JsonPropertyName("Id")]
    public int ServerRef { get; set; }

    public string Type { get; set; }

    [JsonIgnore]
    public string FileName { get; set; }

    [NotMapped]
    public string S3SignedUrl { get; set; }

    public string S3Url { get; set; }

    public int? BaseVisitId { get; set; }
    public int? VisitId { get; set; }
    public int? VisitTaskId { get; set; }
    public int? VisitMedicationId { get; set; }
    public int? VisitFluidId { get; set; }
    public int? BodyMapId { get; set; }
    public int? IncidentId { get; set; }

    [JsonIgnore]
    public long AddedOnTicks { get; set; }

    [Ignored]
    public DateTime AddedOn
    {
        get { return new DateTime(AddedOnTicks); }
        set { AddedOnTicks = value.Ticks; }
    }
}