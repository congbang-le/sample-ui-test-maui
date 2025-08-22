namespace VisitTracker;

/// <summary>
/// This class is used to convert a null value to an empty symbol for UI elements.
/// It returns "-" for null values and the original value for non-null values.
/// </summary>
public class NullToEmptySymbolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return "-";

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}