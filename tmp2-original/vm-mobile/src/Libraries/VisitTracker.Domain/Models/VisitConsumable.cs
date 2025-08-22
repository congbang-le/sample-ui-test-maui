namespace VisitTracker.Domain;

public class VisitConsumable : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("MobileRef")]
    public int Id { get; set; } = new Random().Next();

    [JsonPropertyName("Id")]
    public int ServerRef { get; set; }

    public int VisitId { get; set; }

    public int ConsumableTypeId { get; set; }

    public string ConsumableTypeStr { get; set; }

    public int? ReservedQuantity { get; set; }
    public int? QuantityUsed { get; set; }
}