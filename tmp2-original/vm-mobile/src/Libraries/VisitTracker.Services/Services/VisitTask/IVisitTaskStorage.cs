namespace VisitTracker.Services;

public interface IVisitTaskStorage : IBaseStorage<VisitTask>
{
    Task<VisitTask> GetByTaskAndVisitId(int taskId, int visitId);

    Task<IList<VisitTask>> GetAllByVisitId(int visitId);
}