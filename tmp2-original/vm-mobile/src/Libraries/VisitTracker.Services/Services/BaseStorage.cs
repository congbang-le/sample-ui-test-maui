namespace VisitTracker.Services;

public class BaseStorage<T> : RealmStorage, IBaseStorage<T> where T : RealmObject, IBaseModel
{
    private readonly ISecuredKeyProvider _keyProvider;

    public BaseStorage(ISecuredKeyProvider keyProvider)
    {
        _keyProvider = keyProvider;
    }

    protected override async Task<RealmConfiguration> GetRealmConfigurationAsync()
    {
        var config = new RealmConfiguration(typeof(T).FullName + ".realm")
        {
            SchemaVersion = 1L,
            ShouldDeleteIfMigrationNeeded = false,
            Schema = new[] { typeof(T) }
        };

        config.EncryptionKey = await _keyProvider.GetKey(typeof(T).FullName + ".key", 64);
        return config;
    }

    public async Task<IList<T>> Select(Query<T> query)
    {
        return await base.Select<T>(query);
    }

    public async Task<T> FirstOrDefault(Query<T> query)
    {
        return await base.FirstOrDefault<T>(query);
    }

    public async Task<IList<T>> GetAll()
    {
        return await base.List<T>();
    }

    public async Task<IList<T>> GetAllByIds(IEnumerable<int> ids)
    {
        return await base.ListByIds<T>(ids);
    }

    public async Task<IList<T>> GetAllByIds(IEnumerable<int> ids, string column)
    {
        return await base.ListByIds<T>(ids, column);
    }

    public async Task<T> GetById(int id)
    {
        return await base.Find<T>(id);
    }

    public async Task<T> InsertOrReplace(T entity)
    {
        return await base.InsertOrReplace(entity);
    }

    public async Task<IList<T>> InsertOrReplace(IList<T> entities)
    {
        return await base.InsertOrReplace(entities);
    }

    public async Task<int> DeleteAll()
    {
        return await base.DeleteAll<T>();
    }

    public async Task<int> DeleteAllByIds(IEnumerable<int> ids)
    {
        return await base.DeleteAllByIds<T>(ids);
    }

    public async Task<int> DeleteAllByIds(IEnumerable<int> ids, string column)
    {
        return await base.DeleteAllByIds<T>(ids, column);
    }

    public async Task<int> DeleteAllById(int id)
    {
        return await base.Delete<T>(id);
    }
}