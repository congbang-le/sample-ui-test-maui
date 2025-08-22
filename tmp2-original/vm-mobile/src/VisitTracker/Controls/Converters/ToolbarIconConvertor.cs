namespace VisitTracker;

/// <summary>
/// This class is used to convert a boolean value to a FontImageSource for toolbar icons.
/// It uses the MaterialCommunityIcons font family and a specific glyph for the icon.
/// </summary>
public class ToolbarIconConvertor : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 1) return null;

        if (values[0] is bool isEnabled && isEnabled)
        {
            return new FontImageSource
            {
                FontFamily = "MaterialCommunityIcons",
                Glyph = "\uF90B",
                Size = 29,
                Color = isEnabled ? Colors.White.WithAlpha(0.7f) : Colors.White.WithAlpha(0.5f)
            };
        }

        return new FontImageSource
        {
            FontFamily = "MaterialCommunityIcons",
            Glyph = "",
            Size = 28,
            Color = Colors.White.WithAlpha(0.5f)
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}