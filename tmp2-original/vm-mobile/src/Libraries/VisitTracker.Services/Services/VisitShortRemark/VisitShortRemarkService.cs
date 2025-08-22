namespace VisitTracker.Services;

public class VisitShortRemarkService : BaseService<VisitShortRemark>, IVisitShortRemarkService
{
    private readonly IVisitShortRemarkStorage _storage;

    public VisitShortRemarkService(IVisitShortRemarkStorage visitShortRemarkStorage) : base(visitShortRemarkStorage)
    {
        _storage = visitShortRemarkStorage;
    }

    public async Task<IList<VisitShortRemark>> GetAllByVisitId(int id)
    {
        return await _storage.GetAllByVisitId(id);
    }

    public async Task DeleteAllByVisitId(int id)
    {
        await _storage.DeleteAllByVisitId(id);
    }
}