namespace VisitTracker.Services;

public interface ISyncApi
{
    Task<SyncResponse> SyncDataToServer(IEnumerable<Sync> data);
}