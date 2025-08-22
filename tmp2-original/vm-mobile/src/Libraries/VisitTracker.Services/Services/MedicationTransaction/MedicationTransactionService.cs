namespace VisitTracker.Services;

public class MedicationTransactionService : BaseService<MedicationTransaction>, IMedicationTransactionService
{
    private readonly IMedicationTransactionStorage _storage;

    public MedicationTransactionService(IMedicationTransactionStorage medicationTransactionStorage) : base(medicationTransactionStorage)
    {
        _storage = medicationTransactionStorage;
    }

    public async Task DeleteAllByBookingId(int bookingId)
    {
        await _storage.DeleteAllByBookingId(bookingId);
    }
}