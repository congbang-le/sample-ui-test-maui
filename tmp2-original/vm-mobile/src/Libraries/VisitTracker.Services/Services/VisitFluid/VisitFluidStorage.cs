namespace VisitTracker.Services;

public class VisitFluidStorage : BaseStorage<VisitFluid>, IVisitFluidStorage
{
    public VisitFluidStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<VisitFluid> GetByVisitId(int id)
    {
        return await FirstOrDefault(q => q.Where(x => x.VisitId == id));
    }

    public async Task DeleteByVisitId(int id)
    {
        var result = await Select(q => q.Where(x => x.VisitId == id));
        if (result != null && result.Any())
            await DeleteAllByIds(result.Select(i => i.Id));
    }
}