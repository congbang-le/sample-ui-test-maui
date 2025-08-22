namespace VisitTracker.Domain;

public class MedicationTransaction : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public int MedicationId { get; set; }

    public int? ResponseId { get; set; }

    [JsonIgnore]
    public long TransactionTimeTicks { get; set; }

    [Ignored]
    public DateTime TransactionTime
    {
        get { return new DateTime(TransactionTimeTicks); }
        set { TransactionTimeTicks = value.Ticks; }
    }
}