namespace VisitTracker.Services;

public class VisitHealthStatusService : BaseService<VisitHealthStatus>, IVisitHealthStatusService
{
    private readonly IVisitHealthStatusStorage _storage;

    public VisitHealthStatusService(IVisitHealthStatusStorage visitHealthStatusStorage) : base(visitHealthStatusStorage)
    {
        _storage = visitHealthStatusStorage;
    }

    public async Task<IList<VisitHealthStatus>> GetAllByVisitId(int id)
    {
        return await _storage.GetAllByVisitId(id);
    }

    public async Task DeleteAllByVisitId(int id)
    {
        await _storage.DeleteAllByVisitId(id);
    }
}