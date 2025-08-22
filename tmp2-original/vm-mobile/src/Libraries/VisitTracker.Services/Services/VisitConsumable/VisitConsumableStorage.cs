namespace VisitTracker.Services;

public class VisitConsumableStorage : BaseStorage<VisitConsumable>, IVisitConsumableStorage
{
    public VisitConsumableStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<VisitConsumable>> GetAllByVisitId(int id)
    {
        return await Select(q => q.Where(x => x.VisitId == id));
    }

    public async Task DeleteAllByVisitId(int id)
    {
        var consumables = await Select(q => q.Where(x => x.VisitId == id));
        if (consumables != null && consumables.Any())
            await DeleteAllByIds(consumables.Select(x => x.Id));
    }
}