namespace VisitTracker.Domain;

public class Visit : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public int BookingDetailId { get; set; }
    public int VisitStatusId { get; set; }

    public string Summary { get; set; }
    public string HandOverNotes { get; set; }
    public bool IsCompleted { get; set; }

    public string SensorsNotFound { get; set; }
    public string MachineInfo { get; set; }
    public string DeviceInfo { get; set; }

    [JsonIgnore]
    public long StartedOnTicks { get; set; }

    [Ignored]
    public DateTime? StartedOn
    {
        get { return StartedOnTicks != 0 ? new DateTime(StartedOnTicks) : null; }
        set { StartedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    [JsonIgnore]
    public long AcknowledgedOnTicks { get; set; }

    [Ignored]
    public DateTime? AcknowledgedOn
    {
        get { return AcknowledgedOnTicks != 0 ? new DateTime(AcknowledgedOnTicks) : null; }
        set { AcknowledgedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    public string StartedLocation { get; set; }
    public string UploadedLocation { get; set; }
    public string TerminationReason { get; set; }
    public bool IsVisitTampered { get; set; }

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

    public bool IsSynced { get; set; }

    [JsonIgnore]
    [NotMapped]
    public string DisplayName { get; set; }
}