namespace VisitTracker.Domain;

public class Sync : RealmObject, IBaseModel
{
    [PrimaryKey]
    [JsonPropertyName("SyncId")]
    public int Id { get; set; } = new Random().Next();

    public Sync()
    { }

    public string Identifier { get; set; }
    public int IdentifierId { get; set; }
    public string Content { get; set; }

    [JsonIgnore] public bool IsSynced { get; set; }
    [JsonIgnore] public bool IsProcessing { get; set; }
    [JsonIgnore] public string MetaData { get; set; }
    [JsonIgnore] public long ProcessingTimeTicks { get; set; }

    [Ignored]
    public DateTime ProcessingTime
    {
        get { return new DateTime(ProcessingTimeTicks); }
        set { ProcessingTimeTicks = value.Ticks; }
    }
}