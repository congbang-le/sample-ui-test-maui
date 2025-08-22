namespace VisitTracker.Shared;

/// <summary>
/// This class provides extension methods for JSON serialization and deserialization using System.Text.Json.
/// It includes methods to convert JSON strings to objects and vice versa, with support for case-insensitive property names and custom date handling.
/// </summary>
public static class JsonExtensions
{
    private static JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new DateTimeExtensions.DateTimeConverter() }
    };

    /// <summary>
    /// Deserializes a JSON string into an object of type T.
    /// This method uses System.Text.Json for deserialization and supports case-insensitive property names.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">serialized json string</param>
    /// <returns>object of type T</returns>
    public static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <summary>
    /// Serializes an object into a JSON string.
    /// This method uses System.Text.Json for serialization and supports case-insensitive property names.
    /// </summary>
    /// <param name="json">object to be serialized</param>
    /// <returns>serialized json string</returns>
    public static string Serialize(object json)
    {
        return JsonSerializer.Serialize(json, options);
    }
}