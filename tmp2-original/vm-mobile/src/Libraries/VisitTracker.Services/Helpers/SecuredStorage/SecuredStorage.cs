namespace VisitTracker.Services;

public class SecuredStorage : ISecuredStorage
{
    private static readonly IDictionary<string, string> _memCache = new Dictionary<string, string>();

    public async Task<string> GetAsync(string key)
    {
        if (_memCache.TryGetValue(key, out var value)) return _memCache[key];

        var result = await SecureStorage.Default.GetAsync(key);
        _memCache[key] = result;

        if (result.IsBase64())
        {
            var resultDecrepted = Encryptor.Decrypt(result);
            _memCache[key] = resultDecrepted;
            return resultDecrepted;
        }

        return result;
    }

    public async Task<bool> SetAsync(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var encryptedValue = Encryptor.Encrypt(value);
            _memCache[key] = value;
            var existing = await SecureStorage.Default.GetAsync(key);
            if (existing != encryptedValue)
                await SecureStorage.Default.SetAsync(key, encryptedValue);
        }
        else
        {
            if (_memCache.ContainsKey(key)) _memCache.Remove(key);
            SecureStorage.Default.Remove(key);
        }

        return true;
    }

    public string Get(string key)
    {
        return GetAsync(key).Result;
    }

    public bool Set(string key, string value)
    {
        return SetAsync(key, value).Result;
    }
}