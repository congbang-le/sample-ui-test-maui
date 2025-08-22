namespace VisitTracker.Services;

public class MarChartResponse
{
    public List<BookingMedication> Medications { get; set; }
    public List<VisitMedicationDto> MedicationVisits { get; set; }
}

public class MedicationEntry
{
    public string MedicationName { get; set; }
    public string Strength { get; set; }
    public string GracePriod { get; set; }
    public string Dosage { get; set; }
    public string AdministrationMode { get; set; }
    public string Route { get; set; }
    public string MealInstructions { get; set; }
    public List<MedicationTimeSlot> MedicationTimeSlots { get; set; }
}

public class MedicationTimeSlot
{
    public string Time { get; set; }
    public List<MedicationDetailEntry> MedicationDetails { get; set; }
}

public class MedicationDetailEntry
{
    public int Id { get; set; }
    public string Month { get; set; }
    public string Date { get; set; }
    public string Day { get; set; }
    public string BackgroundColor { get; set; }
}

public class MarChartMedicationResponse
{
    public MedicationDetail MedicationDetail { get; set; }
}

public class MedicationDetail
{
    public string TimeSlot { get; set; }
    public string Completion { get; set; }
    public string Summary { get; set; }
    public string CompletionDetail { get; set; }
}

public class VisitMedicationDto
{
    public int Id { get; set; }
    public int MedicationId { get; set; }
    public string BackgroundColor { get; set; }
}