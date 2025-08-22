namespace VisitTracker.Shared;

/// <summary>
/// Extension methods for DateTime operations.
/// This class provides methods to manipulate and format DateTime objects, including converting to and from JSON.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns the current date and time without any timezone information.
    /// </summary>
    /// <returns></returns>
    public static DateTime NowNoTimezone()
    {
        return DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
    }

    /// <summary>
    /// Converts a DateTime object to a string representation in the format "yyyy-MM-ddTHH:mm:ss.fff".
    /// </summary>
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out DateTime result))
                return result;

            return DateTime.MinValue;
        }
    }
}