namespace VisitTracker;

/// <summary>
/// This class is used to convert a string value to a MaterialCommunityIconsFont icon for UI elements.
/// It maps specific string values to corresponding icons.
/// </summary>
public class TypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "Location" => MaterialCommunityIconsFont.MapMarker,
            "Activity" => MaterialCommunityIconsFont.Run,
            "Battery" => MaterialCommunityIconsFont.Battery50,
            "Notification" => MaterialCommunityIconsFont.Bell,
            "Internet" => MaterialCommunityIconsFont.InternetExplorer,
            "ClockTampered" => MaterialCommunityIconsFont.Clock,
            _ => MaterialCommunityIconsFont.SettingsOutline,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}