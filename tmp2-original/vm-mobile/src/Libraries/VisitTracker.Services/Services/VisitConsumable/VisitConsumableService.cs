namespace VisitTracker.Services;

public class VisitConsumableService : BaseService<VisitConsumable>, IVisitConsumableService
{
    private readonly IVisitConsumableStorage _storage;

    public VisitConsumableService(IVisitConsumableStorage visitConsumableStorage) : base(visitConsumableStorage)
    {
        _storage = visitConsumableStorage;
    }

    public async Task<IList<VisitConsumable>> GetAllByVisitId(int id)
    {
        return await _storage.GetAllByVisitId(id);
    }

    public async Task DeleteAllByVisitId(int id)
    {
        await _storage.DeleteAllByVisitId(id);
    }
}