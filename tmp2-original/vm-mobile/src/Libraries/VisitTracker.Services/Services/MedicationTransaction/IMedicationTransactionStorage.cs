namespace VisitTracker.Services;

public interface IMedicationTransactionStorage : IBaseStorage<MedicationTransaction>
{
    Task DeleteAllByBookingId(int bookingId);
}