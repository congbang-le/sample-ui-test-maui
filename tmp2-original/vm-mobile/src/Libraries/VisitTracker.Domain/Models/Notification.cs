namespace VisitTracker.Domain;

public class Notification : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public bool IsSynced { get; set; }

    public int? TypeId { get; set; }

    public string Title { get; set; }
    public string Message { get; set; }

    public int NotificationTypeId { get; set; }
    public string NotificationType { get; set; }

    public string Content { get; set; }
    public string Icon { get; set; }
    public string Color { get; set; }

    public bool RequireAcknowledgement { get; set; }
    public bool IsAcknowledged { get; set; }

    public int? ExternalNotificationId { get; set; }
    public bool SuppressNotification { get; set; }

    public string Data { get; set; }

    [JsonIgnore]
    public long AcknowledgedTimeTicks { get; set; }

    [Ignored]
    public DateTime? AcknowledgedTime
    {
        get { return AcknowledgedTimeTicks != 0 ? new DateTime(AcknowledgedTimeTicks) : null; }
        set { AcknowledgedTimeTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    [JsonIgnore]
    public long CreatedTimeTicks { get; set; }

    [Ignored]
    public DateTime? CreatedTime
    {
        get { return CreatedTimeTicks != 0 ? new DateTime(CreatedTimeTicks) : null; }
        set { CreatedTimeTicks = value.HasValue ? value.Value.Ticks : default; }
    }
}