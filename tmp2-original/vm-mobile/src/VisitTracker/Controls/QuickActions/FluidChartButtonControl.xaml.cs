namespace VisitTracker;

public partial class FluidChartButtonControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenFluidChartCommand { get; set; }

    public FluidChartButtonControl()
    {
        InitializeComponent();

        OpenFluidChartCommand = ReactiveCommand.CreateFromTask(OpenFluidChart);
        BindBusy(OpenFluidChartCommand);
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
        typeof(int), typeof(FluidChartButtonControl), defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: ServiceUserIdPropertyChanged);

    private static void ServiceUserIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FluidChartButtonControl)bindable;
        control.ServiceUserId = Convert.ToInt32(newValue);
    }

    protected async Task OpenFluidChart()
    {
        var fluidChartPage = $"{nameof(FluidChartPage)}?Id={ServiceUserId}";
        await Shell.Current.GoToAsync(fluidChartPage);
    }
}