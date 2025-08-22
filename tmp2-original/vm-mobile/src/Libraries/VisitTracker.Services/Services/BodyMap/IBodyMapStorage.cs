namespace VisitTracker.Services;

public interface IBodyMapStorage : IBaseStorage<BodyMap>
{
    Task<IList<BodyMap>> GetAllByBaseVisitId(int id);

    Task<IList<BodyMap>> GetAllByVisitId(int id);

    Task<IList<BodyMap>> GetAllByTaskId(int id);

    Task<IList<BodyMap>> GetAllByMedicationId(int id);

    Task<IList<BodyMap>> GetAllByFluidId(int id);

    Task<IList<BodyMap>> GetAllByIncidentId(int id);

    Task<BodyMap> GetByBookingId(int id);

    Task<BodyMap> GetByTaskId(int id);

    Task<BodyMap> GetByMedicationId(int id);

    Task<BodyMap> GetByFluidId(int id);

    Task<BodyMap> GetByIncidentId(int id);

    Task DeleteAllByVisitId(int id);

    Task DeleteAllByIncidentId(int id);
}