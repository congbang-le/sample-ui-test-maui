namespace VisitTracker;

public class ShowAlertIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value?.ToString()?.ToLower();
        return str == "true" || str == "null";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "True";
    }
}


