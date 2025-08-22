namespace VisitTracker.Services;

/// <summary>
/// Interface for managing storage operations for a specific type of object T.
/// This interface provides methods for retrieving, inserting, and deleting objects in the storage system.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBaseService<T>
{
    Task<T> GetById(int id);

    Task<IList<T>> GetAll();

    Task<int> DeleteAllById(int id);

    Task<int> DeleteAll();

    Task<T> InsertOrReplace(T obj);

    Task<IList<T>> InsertOrReplace(IList<T> objList);

    Task<IList<T>> GetAllByIds(IEnumerable<int> ids);

    Task<IList<T>> GetAllByIds(IEnumerable<int> ids, string column);

    Task<int> DeleteAllByIds(IEnumerable<int> ids);

    Task<int> DeleteAllByIds(IEnumerable<int> ids, string column);
}