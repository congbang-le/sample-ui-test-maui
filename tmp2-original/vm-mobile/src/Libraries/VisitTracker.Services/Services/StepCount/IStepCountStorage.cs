namespace VisitTracker.Services;

public interface IStepCountStorage : IBaseStorage<VisitStepCount>
{
    Task<IList<VisitStepCount>> GetAllByVisitId(int id);

    Task DeleteAllByVisitId(int bookingId);

    Task<IList<VisitStepCount>> GetAllBySupVisitId(int id);
}