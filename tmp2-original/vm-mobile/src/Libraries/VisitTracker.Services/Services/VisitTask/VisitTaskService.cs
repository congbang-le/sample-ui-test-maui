namespace VisitTracker.Services;

public class VisitTaskService : BaseService<VisitTask>, IVisitTaskService
{
    private readonly IVisitTaskStorage _storage;

    public VisitTaskService(IVisitTaskStorage taskStorage) : base(taskStorage)
    {
        _storage = taskStorage;
    }

    public async Task<VisitTask> GetByTaskAndVisitId(int taskId, int visitId)
    {
        return await _storage.GetByTaskAndVisitId(taskId, visitId);
    }

    public async Task<IList<VisitTask>> GetAllByVisitId(int visitId)
    {
        return await _storage.GetAllByVisitId(visitId);
    }
}