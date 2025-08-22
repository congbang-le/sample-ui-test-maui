namespace VisitTracker.Services;

public class VisitFluidService : BaseService<VisitFluid>, IVisitFluidService
{
    private readonly IVisitFluidStorage _storage;

    public VisitFluidService(IVisitFluidStorage visitFluidStorage) : base(visitFluidStorage)
    {
        _storage = visitFluidStorage;
    }

    public async Task<VisitFluid> GetByVisitId(int id)
    {
        return await _storage.GetByVisitId(id);
    }

    public async Task DeleteByVisitId(int id)
    {
        await _storage.DeleteByVisitId(id);
    }
}