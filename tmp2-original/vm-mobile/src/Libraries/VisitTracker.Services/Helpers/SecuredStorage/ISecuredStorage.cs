namespace VisitTracker.Services;

/// <summary>
/// Interface for managing secured storage operations.
/// This interface provides methods for getting and setting secured data using a key-value pair approach.
/// </summary>
public interface ISecuredStorage
{
    string Get(string key);

    bool Set(string key, string value);

    Task<string> GetAsync(string key);

    Task<bool> SetAsync(string key, string value);
}