namespace VisitTracker.Services;

public interface ISyncService : IBaseService<Sync>
{
    Task<bool> DeleteAllBySyncData();

    Task<(List<Sync>, SyncResponse)> SyncData();

    Task<IList<Sync>> GetAllUnSynced();

    Task DeleteByIdentifierId(int identifierId);
}