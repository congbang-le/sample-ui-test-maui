namespace VisitTracker.Domain;

public class SupervisorVisit : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public int ServiceUserId { get; set; }
    public int SupervisorId { get; set; }

    [JsonIgnore]
    public long StartedOnTicks { get; set; }

    [Ignored]
    public DateTime? StartedOn
    {
        get { return StartedOnTicks != 0 ? new DateTime(StartedOnTicks) : null; }
        set { StartedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    public string SensorsNotFound { get; set; }

    public bool IsCompleted { get; set; }

    public string MachineInfo { get; set; }
    public string DeviceInfo { get; set; }

    public string TerminationReason { get; set; }

    [JsonIgnore]
    public long TerminatedOnTicks { get; set; }

    [Ignored]
    public DateTime? TerminatedOn
    {
        get { return TerminatedOnTicks != 0 ? new DateTime(TerminatedOnTicks) : null; }
        set { TerminatedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    [JsonIgnore]
    public long CompletedOnTicks { get; set; }

    [Ignored]
    public DateTime? CompletedOn
    {
        get { return CompletedOnTicks != 0 ? new DateTime(CompletedOnTicks) : null; }
        set { CompletedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    public string UploadedLocation { get; set; }
    public string StartedLocation { get; set; }

    public bool? IsVisitTampered { get; set; }
}