namespace VisitTracker.Services;

public delegate T Transaction<T>(Realm realm);

public delegate IQueryable<T> Query<T>(IQueryable<T> query);

public abstract class RealmStorage : IStorage
{
    protected async Task<Realm> GetRealm()
    {
        var config = await GetRealmConfigurationAsync();
        return await Realm.GetInstanceAsync(config);
    }

    protected abstract Task<RealmConfiguration> GetRealmConfigurationAsync();

    public async Task<IList<T>> List<T>() where T : RealmObject
    {
        var realm = await GetRealm();
        return realm.All<T>()
            .ToList()
            .Select(e => e.Copy())
            .ToList();
    }

    public async Task<IList<T>> ListByIds<T>(IEnumerable<int> ids) where T : RealmObject, IBaseModel
    {
        var realm = await GetRealm();

        var queryString = string.Join(" OR ", ids.Select(i => $"Id == {i}"));
        if (string.IsNullOrEmpty(queryString))
            return default;

        return (await Select<T>(query => { return query.Filter(queryString); }))
            .ToList()
            .Select(e => e.Copy())
            .ToList();
    }

    public async Task<IList<T>> ListByIds<T>(IEnumerable<int> ids, string column) where T : RealmObject, IBaseModel
    {
        var realm = await GetRealm();

        var queryString = string.Join(" OR ", ids.Select(i => column + $" == {i}"));
        if (string.IsNullOrEmpty(queryString))
            return default;

        return (await Select<T>(query => { return query.Filter(queryString); }))
            .ToList()
            .Select(e => e.Copy())
            .ToList();
    }

    public async Task<T> Find<T>(int id) where T : RealmObject
    {
        var realm = await GetRealm();
        var entity = realm.Find<T>(id);
        return entity?.Copy();
    }

    public async Task<int> Delete<T>(int id) where T : RealmObject
    {
        var realm = await GetRealm();

        var entity = realm.Find<T>(id);
        if (entity != null)
            realm.Write(() => { realm.Remove(entity); });

        return entity == null ? 0 : 1;
    }

    public async Task<T> InsertOrReplace<T>(T entity) where T : RealmObject
    {
        return await ExecuteInTransaction(realm => { return realm.Add(entity, true).Copy(); });
    }

    public async Task<IList<T>> InsertOrReplace<T>(IList<T> entities) where T : RealmObject
    {
        return await ExecuteInTransaction(realm =>
        {
            var result = new List<T>(entities.Count);
            foreach (var entity in entities)
                result.Add(realm.Add(entity, true).Copy());

            return result;
        });
    }

    public async Task<int> DeleteAll<T>() where T : RealmObject
    {
        return await ExecuteInTransaction(realm =>
        {
            var query = realm.All<T>();
            var count = query.Count();
            realm.RemoveRange(query);
            return count;
        });
    }

    public async Task<int> DeleteAllByIds<T>(IEnumerable<int> ids) where T : RealmObject, IBaseModel
    {
        var queryString = string.Join(" OR ", ids.Select(i => $"Id == {i}"));
        if (string.IsNullOrEmpty(queryString))
            return default;

        return await Delete<T>(query => { return query.Filter(queryString); });
    }

    public async Task<int> DeleteAllByIds<T>(IEnumerable<int> ids, string column) where T : RealmObject, IBaseModel
    {
        var queryString = string.Join(" OR ", ids.Select(i => column + $" == {i}"));
        if (string.IsNullOrEmpty(queryString))
            return default;

        return await Delete<T>(query => { return query.Filter(queryString); });
    }

    public async Task<IList<T>> Select<T>(Query<T> query) where T : RealmObject
    {
        var realm = await GetRealm();
        var queryable = realm.All<T>();
        return query.Invoke(queryable)
            .ToList()
            .Select(e => e.Copy())
            .ToList();
    }

    public async Task<T> FirstOrDefault<T>(Query<T> query) where T : RealmObject
    {
        var realm = await GetRealm();
        var queryable = realm.All<T>();
        return query.Invoke(queryable)
            .ToList()
            .Select(e => e.Copy())
            .FirstOrDefault();
    }

    public async Task<int> Delete<T>(Query<T> criteria) where T : RealmObject
    {
        return await ExecuteInTransaction(realm =>
        {
            var query = criteria.Invoke(realm.All<T>());
            var count = query.Count();
            realm.RemoveRange(query);

            return count;
        });
    }

    public async Task<T> ExecuteInTransaction<T>(Transaction<T> trx)
    {
        var result = default(T);

        var realm = await GetRealm();
        await realm.WriteAsync(() => { result = trx.Invoke(realm); });

        return result;
    }
}