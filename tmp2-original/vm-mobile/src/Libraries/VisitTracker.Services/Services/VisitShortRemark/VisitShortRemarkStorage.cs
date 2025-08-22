namespace VisitTracker.Services;

public class VisitShortRemarkStorage : BaseStorage<VisitShortRemark>, IVisitShortRemarkStorage
{
    public VisitShortRemarkStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<VisitShortRemark>> GetAllByVisitId(int id)
    {
        return await Select(q => q.Where(x => x.VisitId == id));
    }

    public async Task DeleteAllByVisitId(int id)
    {
        var ids = (await Select(q => q.Where(x => x.VisitId == id))).Select(x => x.Id);
        if (ids != null && ids.Any())
            await DeleteAllByIds(ids);
    }
}