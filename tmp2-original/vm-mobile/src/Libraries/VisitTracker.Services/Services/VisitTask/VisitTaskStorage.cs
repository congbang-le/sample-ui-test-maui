namespace VisitTracker.Services;

public class VisitTaskStorage : BaseStorage<VisitTask>, IVisitTaskStorage
{
    public VisitTaskStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<VisitTask> GetByTaskAndVisitId(int taskId, int visitId)
    {
        return await FirstOrDefault(q => q.Where(x => x.TaskId == taskId && x.VisitId == visitId));
    }

    public async Task<IList<VisitTask>> GetAllByVisitId(int visitId)
    {
        return await Select(q => q.Where(x => x.VisitId == visitId));
    }
}