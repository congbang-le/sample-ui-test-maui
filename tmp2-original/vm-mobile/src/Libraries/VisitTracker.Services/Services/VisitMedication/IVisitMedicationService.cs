namespace VisitTracker.Services;

public interface IVisitMedicationService : IBaseService<VisitMedication>
{
    Task<VisitMedication> GetByMedicationId(int medicationId);

    Task<VisitMedication> GetByMedicationAndVisitId(int medicationId, int visitId);

    Task<IList<VisitMedication>> GetAllByVisitId(int visitId);
}