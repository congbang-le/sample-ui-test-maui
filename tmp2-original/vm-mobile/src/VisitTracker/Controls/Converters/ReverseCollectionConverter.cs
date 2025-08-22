namespace VisitTracker;

/// <summary>
/// This class is used to reverse a collection for UI elements.
/// </summary>
public class ReverseCollectionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IEnumerable<object> collection)
        {
            return collection.Reverse(); // Reverse the collection
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}