namespace VisitTracker;

public partial class CardViewControl : BaseContentView
{
    public CardViewControl()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty CardTitleProperty = BindableProperty.Create(nameof(CardTitle),
            typeof(string), typeof(CardViewControl),
            defaultValue: string.Empty, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: CardTitlePropertyChanged);

    public static readonly BindableProperty HasCardTitleProperty = BindableProperty.Create(nameof(HasCardTitle),
            typeof(bool), typeof(CardViewControl),
            defaultValue: true, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: HasCardTitlePropertyChanged);

    private static void CardTitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CardViewControl)bindable;
        control.CardTitle = newValue?.ToString();
    }

    private static void HasCardTitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CardViewControl)bindable;
        control.HasCardTitle = (bool)newValue;
    }

    public string CardTitle
    {
        get
        {
            return base.GetValue(CardTitleProperty)?.ToString();
        }
        set
        {
            base.SetValue(CardTitleProperty, value);
        }
    }

    public bool HasCardTitle
    {
        get
        {
            return (bool)base.GetValue(HasCardTitleProperty);
        }
        set
        {
            base.SetValue(HasCardTitleProperty, value);
        }
    }
}