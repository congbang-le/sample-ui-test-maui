namespace VisitTracker.Domain;

public class ServiceUserFp : RealmObject, IBaseModel
{
    [JsonIgnore]
    [PrimaryKey]
    public int Id { get; set; } = new Random().Next();

    public int ServiceUserId { get; set; }

    public string SensorsNotFound { get; set; }
    public string MachineInfo { get; set; }
    public string DeviceInfo { get; set; }

    [JsonIgnore]
    public long StartedOnTicks { get; set; }

    [Ignored]
    public DateTime StartedOn
    {
        get { return new DateTime(StartedOnTicks); }
        set { StartedOnTicks = value.Ticks; }
    }
}