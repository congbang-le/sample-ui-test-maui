namespace VisitTracker.Services;

public interface ISyncStorage : IBaseStorage<Sync>
{
    Task<bool> DeleteAllSync();

    Task<IList<Sync>> GetAllUnSynced();

    Task<IList<Sync>> SetSyncInProcess(IList<Sync> dbSyncs);

    Task DeleteByIdentifierId(int identifierId);
}