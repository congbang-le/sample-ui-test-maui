namespace VisitTracker.Services;

public class VisitMedicationService : BaseService<VisitMedication>, IVisitMedicationService
{
    private readonly IVisitMedicationStorage _storage;

    public VisitMedicationService(IVisitMedicationStorage visitMedicationStorage) : base(visitMedicationStorage)
    {
        _storage = visitMedicationStorage;
    }

    public async Task<VisitMedication> GetByMedicationId(int medicationId)
    {
        return await _storage.GetByMedicationId(medicationId);
    }

    public async Task<VisitMedication> GetByMedicationAndVisitId(int medicationId, int visitId)
    {
        return await _storage.GetByMedicationAndVisitId(medicationId, visitId);
    }

    public async Task<IList<VisitMedication>> GetAllByVisitId(int visitId)
    {
        return await _storage.GetAllByVisitId(visitId);
    }
}