namespace VisitTracker.Services;

public interface IMedicationService : IBaseService<BookingMedication>
{
    Task<IList<BookingMedication>> GetAllByBookingId(int bookingId);

    Task<bool> RequestMedicationAdministration(int medicationId);
}