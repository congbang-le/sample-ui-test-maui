namespace VisitTracker;


public class FpAlertColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = value?.ToString();
        return status switch
        {
            "True" => Application.Current.Resources["AlertWarningTextColor"],
            "Null" => Application.Current.Resources["AlertDangerTextColor"],
            _ => Colors.Transparent
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}