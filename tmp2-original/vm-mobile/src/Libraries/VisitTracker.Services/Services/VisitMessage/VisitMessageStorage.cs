namespace VisitTracker.Services;

public class VisitMessageStorage : BaseStorage<VisitMessage>, IVisitMessageStorage
{
    public VisitMessageStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<VisitMessage>> GetAllByType(EMessageType type)
    {
        var allVisitMessages = await GetAll();
        return allVisitMessages.Where(x => x.Type == type.ToString()).ToList();
    }
}