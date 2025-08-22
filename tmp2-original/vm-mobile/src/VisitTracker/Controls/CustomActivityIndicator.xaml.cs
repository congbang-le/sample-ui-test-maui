using SkiaSharp.Extended.UI.Controls;

namespace VisitTracker;

public partial class CustomActivityIndicator : BaseContentView
{
    public CustomActivityIndicator()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SourceFileProperty = BindableProperty.Create(nameof(SourceFile),
            typeof(SKLottieImageSource), typeof(CustomActivityIndicator),
            defaultValue: new SKFileLottieImageSource { File = "LoaderBlue.json" },
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: SourceFilePropertyChanged);

    public static readonly BindableProperty HeightWidthProperty = BindableProperty.Create(nameof(HeightWidth),
            typeof(int), typeof(CustomActivityIndicator),
            defaultValue: 30, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: HeightWidthPropertyChanged);

    public static readonly BindableProperty BusyTextProperty = BindableProperty.Create(nameof(BusyText),
            typeof(string), typeof(CustomActivityIndicator),
            defaultValue: "", defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: BusyTextPropertyChanged);

    public static readonly BindableProperty BusyTextColorProperty = BindableProperty.Create(nameof(BusyTextColor),
            typeof(Color), typeof(CustomActivityIndicator),
            defaultValue: Color.FromArgb(colorAsHex: "#0064F7"),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: BusyTextColorPropertyChanged);

    public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy),
            typeof(bool), typeof(CustomActivityIndicator),
            defaultValue: false, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsBusyPropertyChanged);

    private static void SourceFilePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomActivityIndicator)bindable;
        control.SourceFile = newValue as SKLottieImageSource;
    }

    private static void HeightWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomActivityIndicator)bindable;
        control.HeightWidth = (int)newValue;
    }

    private static void BusyTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomActivityIndicator)bindable;
        control.BusyText = newValue?.ToString();
    }

    private static void BusyTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomActivityIndicator)bindable;
        control.BusyTextColor = newValue as Color;
    }

    private static void IsBusyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomActivityIndicator)bindable;
        control.IsBusy = (bool)newValue;
    }

    public SKLottieImageSource SourceFile
    {
        get
        {
            return (SKLottieImageSource)base.GetValue(SourceFileProperty);
        }
        set
        {
            base.SetValue(SourceFileProperty, value);
        }
    }

    public int HeightWidth
    {
        get
        {
            return (int)base.GetValue(HeightWidthProperty);
        }
        set
        {
            base.SetValue(HeightWidthProperty, value);
        }
    }

    public string BusyText
    {
        get
        {
            return base.GetValue(BusyTextProperty)?.ToString();
        }
        set
        {
            base.SetValue(BusyTextProperty, value);
        }
    }

    public Color BusyTextColor
    {
        get
        {
            return (Color)base.GetValue(BusyTextColorProperty);
        }
        set
        {
            base.SetValue(BusyTextColorProperty, value);
        }
    }

    public new bool IsBusy
    {
        get
        {
            return (bool)base.GetValue(IsBusyProperty);
        }
        set
        {
            base.SetValue(IsBusyProperty, value);
        }
    }
}