namespace VisitTracker.Services;

public class MedicationService : BaseService<BookingMedication>, IMedicationService
{
    private readonly IMedicationApi _api;
    private readonly IMedicationStorage _storage;

    public MedicationService(IMedicationApi api, IMedicationStorage medicationStorage) : base(medicationStorage)
    {
        _api = api;
        _storage = medicationStorage;
    }

    public async Task<IList<BookingMedication>> GetAllByBookingId(int bookingId)
    {
        return await _storage.GetAllByBookingId(bookingId);
    }

    public async Task<bool> RequestMedicationAdministration(int medicationId)
    {
        return await _api.RequestMedicationAdministration(medicationId);
    }
}