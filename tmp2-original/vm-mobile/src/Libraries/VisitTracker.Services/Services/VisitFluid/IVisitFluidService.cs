namespace VisitTracker.Services;

public interface IVisitFluidService : IBaseService<VisitFluid>
{
    Task<VisitFluid> GetByVisitId(int id);

    Task DeleteByVisitId(int id);
}