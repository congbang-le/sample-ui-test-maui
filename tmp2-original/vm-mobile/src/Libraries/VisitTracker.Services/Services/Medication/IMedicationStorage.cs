namespace VisitTracker.Services;

public interface IMedicationStorage : IBaseStorage<BookingMedication>
{
    Task<IList<BookingMedication>> GetAllByBookingId(int bookingId);

    Task DeleteAllByBookingId(int id);
}