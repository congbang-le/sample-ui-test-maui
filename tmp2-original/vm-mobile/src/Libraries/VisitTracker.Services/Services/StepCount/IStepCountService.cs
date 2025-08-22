namespace VisitTracker.Services;

public interface IStepCountService : IBaseService<VisitStepCount>
{
    Task<IList<VisitStepCount>> GetAllByVisitId(int id);

    Task DeleteAllByVisitId(int bookingId);

    Task<IList<VisitStepCount>> GetBySupVisit(int id);
}