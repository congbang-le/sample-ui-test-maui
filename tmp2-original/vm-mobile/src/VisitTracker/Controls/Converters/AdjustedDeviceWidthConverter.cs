namespace VisitTracker;

/// <summary>
/// This class is used to convert the device width to a value adjusted for UI elements.
/// </summary>
public class AdjustedDeviceWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        var deviceWidthInDp = deviceWidth / DeviceDisplay.MainDisplayInfo.Density;
        return deviceWidthInDp - 145;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class AdjustedDeviceWidthConverterFullWidth : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        var deviceWidthInDp = deviceWidth / DeviceDisplay.MainDisplayInfo.Density;
        return deviceWidthInDp - 100;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}