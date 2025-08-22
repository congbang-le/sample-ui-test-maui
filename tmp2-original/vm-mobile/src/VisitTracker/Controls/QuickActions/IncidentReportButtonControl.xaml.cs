namespace VisitTracker;

public partial class IncidentReportButtonControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenIncidentReportCommand { get; set; }

    public IncidentReportButtonControl()
    {
        InitializeComponent();

        OpenIncidentReportCommand = ReactiveCommand.CreateFromTask(OpenIncidentReport);
        BindBusy(OpenIncidentReportCommand);
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
     typeof(int), typeof(IncidentReportButtonControl), defaultBindingMode: BindingMode.TwoWay,
     propertyChanged: ServiceUserIdPropertyChanged);

    private static void ServiceUserIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (IncidentReportButtonControl)bindable;
        control.ServiceUserId = Convert.ToInt32(newValue);
    }

    public int BaseVisitId
    {
        get
        {
            return Convert.ToInt32(GetValue(BaseVisitIdProperty));
        }
        set
        {
            SetValue(BaseVisitIdProperty, value);
        }
    }

    public static readonly BindableProperty BaseVisitIdProperty = BindableProperty.Create(nameof(BaseVisitId),
     typeof(int), typeof(IncidentReportButtonControl), defaultBindingMode: BindingMode.TwoWay,
     propertyChanged: BaseVisitIdPropertyChanged);

    private static void BaseVisitIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (IncidentReportButtonControl)bindable;
        control.BaseVisitId = Convert.ToInt32(newValue);
    }

    protected async Task OpenIncidentReport()
    {
        if (AppData.Current.CurrentProfile?.Type == nameof(EUserType.SUPERVISOR)
            || (AppData.Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER) && BaseVisitId > 0))
        {
            var navigationParameter = new Dictionary<string, object>
            {
                {nameof(BaseVisitId), BaseVisitId },
                {nameof(ServiceUserId), ServiceUserId }
            };

            var inicdentReportPage = $"{nameof(IncidentReportPage)}?";
            await Shell.Current.GoToAsync(inicdentReportPage, navigationParameter);
        }
        else
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.NoAccessPage, false);
            return;
        }
    }
}