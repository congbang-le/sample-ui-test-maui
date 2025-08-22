namespace VisitTracker;

/// <summary>
/// CollectionViewEx is a custom extended collection view that extends the functionality of the standard CollectionView from MAUI.
/// It provides a bindable property ScrollIndex that allows you to programmatically scroll to a specific index in the collection view.
/// </summary>
public class CollectionViewEx : CollectionView
{
    public static BindableProperty ScrollIndexProperty = BindableProperty.Create(nameof(ScrollIndex),
        typeof(int), typeof(CollectionViewEx), 0, BindingMode.TwoWay, propertyChanged: OnScrollIndexPropertyChanged);

    public int ScrollIndex
    {
        get => (int)base.GetValue(ScrollIndexProperty);
        set => base.SetValue(ScrollIndexProperty, value);
    }

    private static void OnScrollIndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue == null)
            return;

        if (bindable is CollectionViewEx current)
        {
            if (newValue is int scrollIndex)
                current.ScrollTo(scrollIndex, -1, ScrollToPosition.Center, true);
        }
    }
}