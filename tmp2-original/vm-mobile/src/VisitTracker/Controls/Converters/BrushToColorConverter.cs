namespace VisitTracker;

/// <summary>
/// This class is used to convert a SolidColorBrush to a Color value for UI elements.
/// </summary>
public class BrushToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush solidColorBrush)
        {
            return solidColorBrush.Color;
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return new SolidColorBrush(color);
        }
        return null;
    }
}