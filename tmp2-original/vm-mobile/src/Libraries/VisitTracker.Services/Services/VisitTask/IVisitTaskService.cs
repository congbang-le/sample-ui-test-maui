namespace VisitTracker.Services;

public interface IVisitTaskService : IBaseService<VisitTask>
{
    Task<VisitTask> GetByTaskAndVisitId(int taskId, int visitId);

    Task<IList<VisitTask>> GetAllByVisitId(int visitId);
}