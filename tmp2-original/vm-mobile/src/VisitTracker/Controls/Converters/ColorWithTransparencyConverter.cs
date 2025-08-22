namespace VisitTracker;

/// <summary>
/// This class is used to convert a hex color string to a Color value with transparency for UI elements.
/// The alpha value is passed as a parameter and is clamped between 0 and 1.
/// </summary>
public class ColorWithTransparencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hexColor && float.TryParse(parameter as string, NumberStyles.Float, CultureInfo.InvariantCulture, out float alpha))
        {
            alpha = Math.Clamp(alpha, 0f, 1f);
            Color color = Color.FromArgb(hexColor);
            return Color.FromRgba(color.Red, color.Green, color.Blue, alpha);
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}