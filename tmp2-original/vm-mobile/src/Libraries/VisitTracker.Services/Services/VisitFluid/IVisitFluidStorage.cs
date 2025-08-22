namespace VisitTracker.Services;

public interface IVisitFluidStorage : IBaseStorage<VisitFluid>
{
    Task DeleteByVisitId(int id);

    Task<VisitFluid> GetByVisitId(int id);
}