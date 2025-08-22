namespace VisitTracker.Domain;

public class VisitShortRemark : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    [JsonPropertyName("Id")]
    public int ServerRef { get; set; }

    public int VisitId { get; set; }

    public int ShortRemarkId { get; set; }
}