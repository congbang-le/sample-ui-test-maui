using System.Globalization;

namespace VisitTracker.Domain;

public class Booking : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public int? RosterId { get; set; }

    public string BookingCode { get; set; }
    public bool FluidChartsApplicable { get; set; }

    [JsonIgnore]
    public long StartTimeTicks { get; set; }

    [Ignored]
    public DateTime StartTime
    {
        get { return new DateTime(StartTimeTicks); }
        set { StartTimeTicks = value.Ticks; }
    }

    [JsonIgnore]
    public long EndTimeTicks { get; set; }

    [Ignored]
    public DateTime EndTime
    {
        get { return new DateTime(EndTimeTicks); }
        set { EndTimeTicks = value.Ticks; }
    }

    public int BookingStatusId { get; set; }

    public int ServiceUserId { get; set; }

    public string HandOverNotes { get; set; }

    public string BookingTypeStr { get; set; }

    [Ignored]
    [JsonIgnore]
    public string BookingTypeWithCode => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(BookingTypeStr.ToLower(CultureInfo.CurrentCulture)) + " (ID: " + BookingCode + ")";

    [Ignored]
    [JsonIgnore]
    public string BookingFromToTime => $"{StartTime:HH:mm} - {EndTime:HH:mm} hrs, {StartTime:ddd, dd MMM yyyy}";

    [JsonIgnore]
    public long CompletedOnTicks { get; set; }

    [Ignored]
    public DateTime? CompletedOn
    {
        get { return CompletedOnTicks != 0 ? new DateTime(CompletedOnTicks) : null; }
        set { CompletedOnTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    public bool? IsCompleted { get; set; }

    public int? CompletionStatusId { get; set; }

    [Ignored]
    [JsonIgnore]
    public bool IsBookingInScope => DateTimeExtensions.NowNoTimezone() <= EndTime && DateTimeExtensions.NowNoTimezone() >= StartTime.AddMinutes(-Constants.NextBookingScopeInMins);
}