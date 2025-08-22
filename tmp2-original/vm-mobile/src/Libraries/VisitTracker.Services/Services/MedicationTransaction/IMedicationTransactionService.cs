namespace VisitTracker.Services;

public interface IMedicationTransactionService : IBaseService<MedicationTransaction>
{
    Task DeleteAllByBookingId(int bookingId);
}