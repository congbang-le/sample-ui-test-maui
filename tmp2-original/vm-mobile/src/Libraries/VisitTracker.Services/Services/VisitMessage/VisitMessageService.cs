namespace VisitTracker.Services;

public class VisitMessageService : BaseService<VisitMessage>, IVisitMessageService
{
    private readonly IVisitMessageApi _api;
    private readonly IVisitMessageStorage _storage;

    public VisitMessageService(IVisitMessageApi api,
        IVisitMessageStorage VisitMessageStorage) : base(VisitMessageStorage)
    {
        _api = api;
        _storage = VisitMessageStorage;
    }

    public async Task<IList<VisitMessage>> GetAllByType(EMessageType type)
    {
        return await _storage.GetAllByType(type);
    }
}