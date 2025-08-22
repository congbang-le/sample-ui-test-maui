namespace VisitTracker.Services;

public class BaseService<T> : IBaseService<T> where T : RealmObject, IBaseModel
{
    private readonly IBaseStorage<T> _baseStorage;

    public BaseService(IBaseStorage<T> baseStorage) : base()
    {
        _baseStorage = baseStorage;
    }

    public async Task<T> GetById(int id)
    {
        return await _baseStorage.GetById(id);
    }

    public async Task<IList<T>> GetAll()
    {
        return await _baseStorage.GetAll();
    }

    public async Task<int> DeleteAllById(int id)
    {
        return await _baseStorage.DeleteAllById(id);
    }

    public async Task<int> DeleteAll()
    {
        return await _baseStorage.DeleteAll();
    }

    public async Task<IList<T>> InsertOrReplace(IList<T> entities)
    {
        return await _baseStorage.InsertOrReplace(entities);
    }

    public async Task<T> InsertOrReplace(T entity)
    {
        return await _baseStorage.InsertOrReplace(entity);
    }

    public async Task<IList<T>> GetAllByIds(IEnumerable<int> ids)
    {
        return await _baseStorage.GetAllByIds(ids);
    }

    public async Task<IList<T>> GetAllByIds(IEnumerable<int> ids, string column)
    {
        return await _baseStorage.GetAllByIds(ids, column);
    }

    public async Task<int> DeleteAllByIds(IEnumerable<int> ids)
    {
        return await _baseStorage.DeleteAllByIds(ids);
    }

    public async Task<int> DeleteAllByIds(IEnumerable<int> ids, string column)
    {
        return await _baseStorage.DeleteAllByIds(ids, column);
    }
}