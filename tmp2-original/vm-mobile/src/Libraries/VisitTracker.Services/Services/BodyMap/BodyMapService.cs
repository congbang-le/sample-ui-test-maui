namespace VisitTracker.Services;

public class BodyMapService : BaseService<BodyMap>, IBodyMapService
{
    private readonly IBodyMapStorage _storage;

    public BodyMapService(IBodyMapStorage bodyMapStorage) : base(bodyMapStorage)
    {
        _storage = bodyMapStorage;
    }

    public async Task<IList<BodyMap>> GetAllByBaseVisitId(int id)
    {
        return await _storage.GetAllByBaseVisitId(id);
    }

    public async Task<IList<BodyMap>> GetAllByVisitId(int id)
    {
        return await _storage.GetAllByVisitId(id);
    }

    public async Task<IList<BodyMap>> GetAllByVisitTaskId(int id)
    {
        return await _storage.GetAllByTaskId(id);
    }

    public async Task<IList<BodyMap>> GetAllByVisitMedicationId(int id)
    {
        return await _storage.GetAllByMedicationId(id);
    }

    public async Task<IList<BodyMap>> GetAllByFluidId(int id)
    {
        return await _storage.GetAllByFluidId(id);
    }

    public async Task<IList<BodyMap>> GetAllByIncidentId(int id)
    {
        return await _storage.GetAllByIncidentId(id);
    }

    public async Task<BodyMap> GetByBookingId(int id)
    {
        return await _storage.GetByBookingId(id);
    }

    public async Task<BodyMap> GetByTaskId(int id)
    {
        return await _storage.GetByTaskId(id);
    }

    public async Task<BodyMap> GetByMedicationId(int id)
    {
        return await _storage.GetByMedicationId(id);
    }

    public async Task<BodyMap> GetByFluidId(int id)
    {
        return await _storage.GetByFluidId(id);
    }

    public async Task<BodyMap> GetByIncidentId(int id)
    {
        return await _storage.GetByIncidentId(id);
    }

    public async Task DeleteAllByVisitId(int visitId)
    {
        await _storage.DeleteAllByVisitId(visitId);
    }

    public async Task DeleteAllByIncidentId(int incidentId)
    {
        await _storage.DeleteAllByIncidentId(incidentId);
    }
}