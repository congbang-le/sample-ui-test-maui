namespace VisitTracker.Services;

public class VisitHealthStatusStorage : BaseStorage<VisitHealthStatus>, IVisitHealthStatusStorage
{
    public VisitHealthStatusStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<VisitHealthStatus>> GetAllByVisitId(int id)
    {
        return await Select(q => q.Where(x => x.VisitId == id));
    }

    public async Task DeleteAllByVisitId(int id)
    {
        var healthStatuses = await Select(q => q.Where(x => x.VisitId == id));
        if (healthStatuses != null && healthStatuses.Any())
            await DeleteAllByIds(healthStatuses.Select(x => x.Id));
    }
}