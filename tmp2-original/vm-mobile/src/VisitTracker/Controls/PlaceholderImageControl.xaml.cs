namespace VisitTracker;

public partial class PlaceholderImageControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenUserDetailCommand { get; set; }

    public PlaceholderImageControl()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source), typeof(string), typeof(PlaceholderImageControl), default(string),
        propertyChanged: OnSourceChanged);

    public static readonly BindableProperty PlaceholderSourceProperty = BindableProperty.Create(
        nameof(PlaceholderSource), typeof(string), typeof(PlaceholderImageControl), default(string));

    public static readonly BindableProperty SizeOptionProperty = BindableProperty.Create(
        nameof(SizeOption), typeof(string), typeof(PlaceholderImageControl), "Large", propertyChanged: OnSizeOptionChanged);

    private static void OnSizeOptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PlaceholderImageControl control)
        {
            control.UpdateSizeOption();
        }
    }

    public string SizeOption
    {
        get => (string)GetValue(SizeOptionProperty);
        set => SetValue(SizeOptionProperty, value);
    }

    private void UpdateSizeOption()
    {
        if (SizeOption == "Small")
        {
            FrameSize = 32;
            ImageSize = 30;
            CornerRadius = 16;
            ImageCenterPoint = new Point(15, 15);
        }
        else if (SizeOption == "ExtraLarge")
        {
            FrameSize = 92;
            ImageSize = 90;
            CornerRadius = 46;
            ImageCenterPoint = new Point(45, 45);
        }
        else if (SizeOption == "Medium")
        {
            FrameSize = 42;
            ImageSize = 40;
            CornerRadius = 21;
            ImageCenterPoint = new Point(20, 20);
        }
        else if (SizeOption == "Custom")
        {
            FrameSize = 62;
            ImageSize = 60;
            CornerRadius = 31;
            ImageCenterPoint = new Point(30, 30);
        }
        else if (SizeOption == "LargePlus")
        {
            FrameSize = 82;
            ImageSize = 80;
            CornerRadius = 41;
            ImageCenterPoint = new Point(40, 40);
        }
        else
        {
            FrameSize = 52;
            ImageSize = 50;
            CornerRadius = 26;
            ImageCenterPoint = new Point(25, 25);
        }
        HalfImageSize = ImageSize / 2;
    }

    public int FrameSize { get; private set; } = 52;
    public int ImageSize { get; private set; } = 50;
    public int CornerRadius { get; private set; } = 26;
    public int HalfImageSize { get; private set; } = 25;
    public Point ImageCenterPoint { get; private set; } = new Point(25, 25);

    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (PlaceholderImageControl)bindable;
        control.UpdateEffectiveImageSource();
    }

    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string PlaceholderSource
    {
        get => (string)GetValue(PlaceholderSourceProperty);
        set => SetValue(PlaceholderSourceProperty, value);
    }

    private void UpdateEffectiveImageSource()
    {
        EffectiveImageSource = string.IsNullOrEmpty(Source) ? PlaceholderSource : Source;
    }

    public static readonly BindableProperty EffectiveImageSourceProperty = BindableProperty.Create(
        nameof(EffectiveImageSource), typeof(string), typeof(PlaceholderImageControl), default(string));

    public string EffectiveImageSource
    {
        get => (string)GetValue(EffectiveImageSourceProperty);
        private set => SetValue(EffectiveImageSourceProperty, value);
    }
}