namespace VisitTracker.Services;

public class StepCountService : BaseService<VisitStepCount>, IStepCountService
{
    private readonly IStepCountStorage _storage;

    public StepCountService(IStepCountStorage stepCountStorage) : base(stepCountStorage)
    {
        _storage = stepCountStorage;
    }

    public async Task<IList<VisitStepCount>> GetAllByVisitId(int id)
    {
        return await _storage.GetAllByVisitId(id);
    }

    public async Task DeleteAllByVisitId(int id)
    {
        await _storage.DeleteAllByVisitId(id);
    }

    public async Task<IList<VisitStepCount>> GetBySupVisit(int id)
    {
        return await _storage.GetAllBySupVisitId(id);
    }
}