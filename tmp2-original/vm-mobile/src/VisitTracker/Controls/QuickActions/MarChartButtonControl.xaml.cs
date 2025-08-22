namespace VisitTracker;

public partial class MarChartButtonControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenMarChartCommand { get; set; }

    public MarChartButtonControl()
    {
        InitializeComponent();

        OpenMarChartCommand = ReactiveCommand.CreateFromTask(OpenMarChart);
        BindBusy(OpenMarChartCommand);
    }

    public int ServiceUserId
    {
        get
        {
            return Convert.ToInt32(GetValue(ServiceUserIdProperty));
        }
        set
        {
            SetValue(ServiceUserIdProperty, value);
        }
    }

    public static readonly BindableProperty ServiceUserIdProperty = BindableProperty.Create(nameof(ServiceUserId),
        typeof(int), typeof(MarChartButtonControl), defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: ServiceUserIdPropertyChanged);

    private static void ServiceUserIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (MarChartButtonControl)bindable;
        control.ServiceUserId = Convert.ToInt32(newValue);
    }

    protected async Task OpenMarChart()
    {
        var marChartPage = $"{nameof(MarChartPage)}?Id={ServiceUserId}";
        await Shell.Current.GoToAsync(marChartPage);
    }
}