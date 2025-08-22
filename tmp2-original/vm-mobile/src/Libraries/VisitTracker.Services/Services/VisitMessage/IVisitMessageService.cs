namespace VisitTracker.Services;

public interface IVisitMessageService : IBaseService<VisitMessage>
{
    Task<IList<VisitMessage>> GetAllByType(EMessageType type);
}