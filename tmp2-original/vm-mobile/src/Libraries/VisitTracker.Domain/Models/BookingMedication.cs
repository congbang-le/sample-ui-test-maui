namespace VisitTracker.Domain;

public class BookingMedication : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string Strength { get; set; }
    public string Dosage { get; set; }
    public short? Order { get; set; }

    public string DosageUnitStr { get; set; }
    public string AdministrationModeStr { get; set; }
    public string MealInstructionStr { get; set; }
    public string RouteStr { get; set; }
    public string DaysStr { get; set; }

    public int GracePeriod { get; set; }

    public string AllTimeSlot { get; set; }
    public string ApplicableSlot { get; set; }

    [JsonIgnore]
    public long StartDateTimeTicks { get; set; }

    [Ignored]
    public DateTime StartDateTime
    {
        get { return new DateTime(StartDateTimeTicks); }
        set { StartDateTimeTicks = value.Ticks; }
    }

    [JsonIgnore]
    public long EndDateTimeTicks { get; set; }

    [Ignored]
    public DateTime EndDateTime
    {
        get { return new DateTime(EndDateTimeTicks); }
        set { EndDateTimeTicks = value.Ticks; }
    }

    public int ExternalMedicationRef { get; set; }
    public int ExternalMedicationSlotRef { get; set; }
    public short FromHour { get; set; }
    public short ToHour { get; set; }
    public short FromMinute { get; set; }
    public short ToMinute { get; set; }
}