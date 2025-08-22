namespace VisitTracker.Services;

public class BodyMapStorage : BaseStorage<BodyMap>, IBodyMapStorage
{
    public BodyMapStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<BodyMap>> GetAllByBaseVisitId(int id)
    {
        return await Select(q => q.Where(x => x.BaseVisitId == id));
    }

    public async Task<IList<BodyMap>> GetAllByVisitId(int id)
    {
        return await Select(q => q.Where(x => x.VisitId == id));
    }

    public async Task<IList<BodyMap>> GetAllByTaskId(int id)
    {
        return await Select(q => q.Where(x => x.VisitTaskId == id));
    }

    public async Task<IList<BodyMap>> GetAllByMedicationId(int id)
    {
        return await Select(q => q.Where(x => x.VisitMedicationId == id));
    }

    public async Task<IList<BodyMap>> GetAllByFluidId(int id)
    {
        return await Select(q => q.Where(x => x.VisitFluidId == id));
    }

    public async Task<IList<BodyMap>> GetAllByIncidentId(int id)
    {
        return await Select(q => q.Where(x => x.IncidentId == id));
    }

    public async Task<BodyMap> GetByBookingId(int id)
    {
        return await FirstOrDefault(q => q.Where(x => x.IncidentId == id));
    }

    public async Task<BodyMap> GetByTaskId(int id)
    {
        return await FirstOrDefault(q => q.Where(x => x.IncidentId == id));
    }

    public async Task<BodyMap> GetByMedicationId(int id)
    {
        return await FirstOrDefault(q => q.Where(x => x.IncidentId == id));
    }

    public async Task<BodyMap> GetByFluidId(int id)
    {
        return await FirstOrDefault(q => q.Where(x => x.IncidentId == id));
    }

    public async Task<BodyMap> GetByIncidentId(int id)
    {
        return await FirstOrDefault(q => q.Where(x => x.IncidentId == id));
    }

    public async Task DeleteAllByVisitId(int id)
    {
        var bodyMaps = await Select(q => q.Where(x => x.VisitId == id));
        if (bodyMaps != null && bodyMaps.Any())
            await DeleteAllByIds(bodyMaps.Select(x => x.Id));
    }

    public async Task DeleteAllByIncidentId(int id)
    {
        var bodyMaps = await Select(q => q.Where(x => x.IncidentId == id));
        if (bodyMaps != null && bodyMaps.Any())
            await DeleteAllByIds(bodyMaps.Select(x => x.Id));
    }
}