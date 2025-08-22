namespace VisitTracker;

/// <summary>
/// This class is used to convert a title string to a simplified version for UI elements.
/// It removes the first word and keeps the rest of the title.
/// </summary>
public class SimplifyTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string title)
        {
            var words = title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 1)
            {
                return string.Join(" ", words[0], string.Join(" ", words.Skip(2)));
            }
            return title;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}