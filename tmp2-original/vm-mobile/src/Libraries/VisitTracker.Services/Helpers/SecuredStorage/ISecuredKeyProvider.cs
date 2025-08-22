namespace VisitTracker.Services;

/// <summary>
/// Interface for providing secured keys for cryptographic operations.
/// This interface defines a method to retrieve a secured key based on a specified key and length.
/// </summary>
public interface ISecuredKeyProvider
{
    Task<byte[]> GetKey(string key, int length);
}

public class SecuredKeyProvider : ISecuredKeyProvider
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    private static readonly Lazy<IDictionary<string, byte[]>> Cache =
        new(() => { return new Dictionary<string, byte[]>(); });

    private readonly ISecuredStorage _securedStorage;

    public SecuredKeyProvider(ISecuredStorage securedStorage)
    {
        _securedStorage = securedStorage;
    }

    public async Task<byte[]> GetKey(string key, int length)
    {
        await Semaphore.WaitAsync();
        try
        {
            if (Cache.Value.TryGetValue(key, out var secureKey))
                return secureKey;

            var storedKey = await _securedStorage.GetAsync(key);
            if (string.IsNullOrEmpty(storedKey))
            {
                var random = new Random();

                var buffer = new byte[length];
                random.NextBytes(buffer);

                storedKey = Convert.ToBase64String(buffer);

                await _securedStorage.SetAsync(key, storedKey);
            }

            var result = Convert.FromBase64String(storedKey);
            Cache.Value[key] = result;

            return result;
        }
        finally
        {
            Semaphore.Release();
        }
    }
}