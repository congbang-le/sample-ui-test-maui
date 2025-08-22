using System.Net;

namespace VisitTracker.Services;

public class SyncService : BaseService<Sync>, ISyncService
{
    private readonly ISyncApi _api;
    private readonly ISyncStorage _syncStorage;

    public SyncService(ISyncStorage syncStorage, ISyncApi api) : base(syncStorage)
    {
        _syncStorage = syncStorage;
        _api = api;
    }

    public async Task<bool> DeleteAllBySyncData()
    {
        return await _syncStorage.DeleteAllSync();
    }

    public async Task<(List<Sync>, SyncResponse)> SyncData()
    {
        var args = await _syncStorage.GetAllUnSynced();
        args = await _syncStorage.SetSyncInProcess(args);
        SyncResponse response = null;

        if (args == null || !args.Any())
            return (null, response);

        try
        {
            response = await _api.SyncDataToServer(args);
        }
        catch (HttpRequestException)
        {
            response = new SyncResponse
            {
                FailIds = args.Select(x => x.Id)?.ToList(),
                SuccessIds = new List<int>()
            };
        }
        catch (WebException)
        {
            response = new SyncResponse
            {
                FailIds = args.Select(x => x.Id)?.ToList(),
                SuccessIds = new List<int>()
            };
        }

        if (args == null || !args.Any() || response == null)
        {
            response = new SyncResponse
            {
                FailIds = args.Select(x => x.Id)?.ToList(),
                SuccessIds = new List<int>()
            };
            return (null, response);
        }

        args.Where(i => response.SuccessIds.Contains(i.Id)).ToList().ForEach(i => i.IsSynced = true);

        var dbSyncList = await InsertOrReplace(args);
        var items = new List<Sync>();
        foreach (var item in args)
        {
            items.Add(new Sync()
            {
                Content = item.Content,
                Identifier = item.Identifier,
                MetaData = item.MetaData,
            });
        }

        return (items, response);
    }

    public async Task<IList<Sync>> GetAllUnSynced()
    {
        return await _syncStorage.GetAllUnSynced();
    }

    public async Task DeleteByIdentifierId(int identifierId)
    {
        await _syncStorage.DeleteByIdentifierId(identifierId);
    }
}