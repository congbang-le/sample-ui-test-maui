namespace VisitTracker.Services;

public class MedicationTransactionStorage : BaseStorage<MedicationTransaction>, IMedicationTransactionStorage
{
    private readonly IMedicationStorage _medicationStorage;

    public MedicationTransactionStorage(ISecuredKeyProvider keyProvider,
        IMedicationStorage medicationStorage) : base(keyProvider)
    {
        _medicationStorage = medicationStorage;
    }

    public async Task DeleteAllByBookingId(int bookingId)
    {
        var medications = await _medicationStorage.GetAllByBookingId(bookingId);
        foreach (var medication in medications)
        {
            var medicationTransactions = await Select(q => q.Where(x => x.MedicationId == medication.Id));
            if (medicationTransactions != null && medicationTransactions.Any())
                await DeleteAllByIds(medicationTransactions.Select(x => x.Id));
        }
    }
}