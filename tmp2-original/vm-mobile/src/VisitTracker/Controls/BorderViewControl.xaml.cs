namespace VisitTracker;

public partial class BorderViewControl : BaseContentView
{
    public BorderViewControl()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor),
            typeof(Color), typeof(BorderViewControl),
            defaultValue: Colors.SlateGray, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: BorderColorPropertyChanged);

    private static void BorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BorderViewControl)bindable;
        control.BorderColor = (Color)newValue;
    }

    public Color BorderColor
    {
        get
        {
            return (Color)base.GetValue(BorderColorProperty);
        }
        set
        {
            base.SetValue(BorderColorProperty, value);
        }
    }
}