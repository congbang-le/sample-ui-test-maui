namespace VisitTracker;

/// <summary>
/// This class is used to convert a nullable boolean value to a string representation for UI elements.
/// It returns "Null" for null values, "True" for true values, and "False" for false values.
/// </summary>
public class NullabeBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return "Null";
        if (value is bool boolean)
            return boolean ? "True" : "False";
        return "False";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}