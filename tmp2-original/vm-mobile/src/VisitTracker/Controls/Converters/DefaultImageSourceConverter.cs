namespace VisitTracker;

/// <summary>
/// This class is used to convert a string value to an ImageSource value for UI elements.
/// It checks if the string is a valid URL or a file path and returns the corresponding ImageSource.
/// </summary>
public class DefaultImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string imageUrl && !string.IsNullOrEmpty(imageUrl))
        {
            if (imageUrl.StartsWith("https") || imageUrl.StartsWith("http"))
                return ImageSource.FromFile(imageUrl);
            else if (!imageUrl.StartsWith("http") || !imageUrl.StartsWith("https"))
                return ImageSource.FromFile(imageUrl);
            else return ImageSource.FromFile("nophoto.png");
        }

        return ImageSource.FromFile("nophoto.png");
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}