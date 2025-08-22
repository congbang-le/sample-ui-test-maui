namespace VisitTracker;

/// <summary>
/// GeolocationExtensions is a static class that provides extension methods for working with geolocation in the application.
/// </summary>
public static class GeolocationExtensions
{
    public static readonly int MauiHighAccuracyLocationTimeoutInSecs = 5;

    /// <summary>
    /// GetBestLocationAsync is an asynchronous method that retrieves the best available location using the Geolocation API.
    /// It uses the GeolocationRequest class to specify the desired accuracy and timeout for the location request.
    /// </summary>
    /// <returns></returns>
    public static async Task<Location> GetBestLocationAsync()
    {
        return await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best,
                TimeSpan.FromSeconds(MauiHighAccuracyLocationTimeoutInSecs)));
    }

    /// <summary>
    /// ToLatLonString is an extension method for the Location class that converts a Location object to a string representation of its latitude and longitude.
    /// The string format is "latitude,longitude". This method is useful for displaying or logging location information in a human-readable format.
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public static string ToLatLonString(this Location location)
    {
        return $"{location.Latitude},{location.Longitude}";
    }
}