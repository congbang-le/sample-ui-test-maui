namespace VisitTracker;

/// <summary>
/// This class is used to convert a collection to a pluralized string representation for UI elements.
/// It returns the singular form of the label if the collection has one item, and the plural form if it has more than one item.
/// </summary>
public class PluralizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var singularLabel = parameter?.ToString();
        var pluralLabel = singularLabel + "s";
        if (value is IEnumerable<object> list)
        {
            return list?.Count() == 1 ? singularLabel : pluralLabel;
        }

        return singularLabel;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}