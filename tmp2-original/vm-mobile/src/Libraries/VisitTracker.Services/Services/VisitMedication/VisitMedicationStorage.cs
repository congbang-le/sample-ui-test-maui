namespace VisitTracker.Services;

public class VisitMedicationStorage : BaseStorage<VisitMedication>, IVisitMedicationStorage
{
    public VisitMedicationStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<VisitMedication> GetByMedicationId(int medicationId)
    {
        return await FirstOrDefault(q => q.Where(x => x.MedicationId == medicationId));
    }

    public async Task<VisitMedication> GetByMedicationAndVisitId(int medicationId, int visitId)
    {
        return await FirstOrDefault(q => q.Where(x => x.MedicationId == medicationId && x.VisitId == visitId));
    }

    public async Task<IList<VisitMedication>> GetAllByVisitId(int visitId)
    {
        return await Select(q => q.Where(x => x.VisitId == visitId));
    }
}