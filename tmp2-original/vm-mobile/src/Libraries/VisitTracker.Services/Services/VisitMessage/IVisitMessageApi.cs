namespace VisitTracker.Services;

public interface IVisitMessageApi
{
    Task<IList<VisitMessage>> SyncVisitMessages();
}