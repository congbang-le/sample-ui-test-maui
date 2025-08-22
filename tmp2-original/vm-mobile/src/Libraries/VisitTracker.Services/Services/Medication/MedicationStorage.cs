namespace VisitTracker.Services;

public class MedicationStorage : BaseStorage<BookingMedication>, IMedicationStorage
{
    public MedicationStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<BookingMedication>> GetAllByBookingId(int bookingId)
    {
        return await Select(q => q.Where(x => x.BookingId == bookingId));
    }

    public async Task DeleteAllByBookingId(int id)
    {
        var result = await Select(q => q.Where(x => x.BookingId == id));
        if (result != null && result.Any())
            await DeleteAllByIds(result.Select(i => i.Id));
    }
}