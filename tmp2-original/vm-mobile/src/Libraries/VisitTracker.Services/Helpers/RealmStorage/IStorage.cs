namespace VisitTracker.Services;

/// <summary>
/// Interface for managing data storage operations using Realm database.
/// This interface provides methods for listing, finding, inserting, replacing, and deleting objects in the database.
/// </summary>
public interface IStorage
{
    Task<IList<T>> List<T>() where T : RealmObject;

    Task<T> Find<T>(int id) where T : RealmObject;

    Task<IList<T>> Select<T>(Query<T> query) where T : RealmObject;

    Task<T> InsertOrReplace<T>(T query) where T : RealmObject;

    Task<IList<T>> InsertOrReplace<T>(IList<T> entities) where T : RealmObject;

    Task<int> Delete<T>(int id) where T : RealmObject;

    Task<int> DeleteAll<T>() where T : RealmObject;

    Task<int> Delete<T>(Query<T> criteria) where T : RealmObject;

    Task<T> ExecuteInTransaction<T>(Transaction<T> trx);
}