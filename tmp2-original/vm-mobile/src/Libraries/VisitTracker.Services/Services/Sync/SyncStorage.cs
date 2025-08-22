namespace VisitTracker.Services;

public class SyncStorage : BaseStorage<Sync>, ISyncStorage
{
    public SyncStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<bool> DeleteAllSync()
    {
        var allSyncs = await GetAll();
        var ids = allSyncs
            .Where(i => i.IsSynced)
            .Select(i => i.Id).ToList();

        await DeleteAllByIds(ids);
        return true;
    }

    public async Task<IList<Sync>> GetAllUnSynced()
    {
        var allSync = await GetAll();
        var result = allSync.ToList()
            .Where(args => !args.IsSynced && (!args.IsProcessing ||
                (args.IsProcessing && args.ProcessingTime.AddMinutes(5) < DateTimeExtensions.NowNoTimezone())))
            .ToList();

        return result;
    }

    public async Task<IList<Sync>> SetSyncInProcess(IList<Sync> dbSyncs)
    {
        foreach (var dbSync in dbSyncs)
        {
            dbSync.IsProcessing = true;
            dbSync.ProcessingTime = DateTimeExtensions.NowNoTimezone();
        }

        return await InsertOrReplace(dbSyncs);
    }

    public async Task DeleteByIdentifierId(int identifierId)
    {
        var recordsToDelete = await Select(q => q.Where(x => x.IdentifierId == identifierId));
        if (recordsToDelete != null && recordsToDelete.Any())
            await DeleteAllByIds(recordsToDelete.Select(x => x.Id));
    }
}