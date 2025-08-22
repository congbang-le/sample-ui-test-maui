namespace VisitTracker.Services;

public class IncidentStorage : BaseStorage<Incident>, IIncidentStorage
{
    public IncidentStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<Incident>> GetAllAdhoc()
    {
        return await Select(q => q.Where(x => x.VisitId == null));
    }

    public async Task<IList<Incident>> GetAllByVisitId(int id)
    {
        return await Select(q => q.Where(x => x.VisitId == id));
    }

    public async Task<Incident> GetByVisitId(int id)
    {
        return await FirstOrDefault(q => q.Where(x => x.VisitId == id));
    }

    public async Task DeleteAllByBookingId(int id)
    {
        var result = await Select(q => q.Where(x => x.VisitId == id));
        if (result != null && result.Any())
            await DeleteAllByIds(result.Select(i => i.Id));
    }
}