namespace VisitTracker.Services;

public interface IVisitMessageStorage : IBaseStorage<VisitMessage>
{
    Task<IList<VisitMessage>> GetAllByType(EMessageType type);
}