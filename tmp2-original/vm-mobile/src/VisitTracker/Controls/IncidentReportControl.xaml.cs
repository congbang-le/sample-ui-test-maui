namespace VisitTracker;

public partial class IncidentReportControl : BaseContentView
{
    public ReactiveCommand<Incident, Unit> OpenByIncidentCommand { get; set; }

    public IncidentReportControl()
    {
        InitializeComponent();

        OpenByIncidentCommand = ReactiveCommand.CreateFromTask<Incident>(OpenByIncident);

        BindBusy(OpenByIncidentCommand);
    }

    public static readonly BindableProperty ServiceUserIdProperty = BindableProperty.Create(nameof(ServiceUserId),
             typeof(int), typeof(IncidentReportControl), defaultBindingMode: BindingMode.TwoWay,
             propertyChanged: ServiceUserIdPropertyChanged);

    public static readonly BindableProperty BaseVisitIdProperty = BindableProperty.Create(nameof(BaseVisitId),
             typeof(int), typeof(IncidentReportControl), defaultBindingMode: BindingMode.TwoWay,
             propertyChanged: BaseVisitIdPropertyChanged);

    public static readonly BindableProperty IncidentReportListProperty = BindableProperty.Create(nameof(IncidentReportList),
            typeof(List<Incident>), typeof(IncidentReportControl),
            defaultValue: null, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IncidentReportListPropertyChanged);

    public static readonly BindableProperty IncidentProperty = BindableProperty.Create(nameof(Incident),
            typeof(Incident), typeof(IncidentReportControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IncidentPropertyChanged);

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly),
            typeof(bool), typeof(IncidentReportControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsReadOnlyPropertyChanged);

    private static void ServiceUserIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (IncidentReportControl)bindable;
        control.ServiceUserId = Convert.ToInt32(newValue);
    }

    private static void BaseVisitIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (IncidentReportControl)bindable;
        control.BaseVisitId = Convert.ToInt32(newValue);
    }

    public static void IncidentReportListPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (IncidentReportControl)bindable;
        control.IncidentReportList = newValue as List<Incident>;
    }

    private static void IncidentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (IncidentReportControl)bindable;
        control.Incident = newValue as Incident;
    }

    private static void IsReadOnlyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (IncidentReportControl)bindable;
        control.IsReadOnly = (bool)newValue;
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

    public List<Incident> IncidentReportList
    {
        get
        {
            return base.GetValue(IncidentReportListProperty) as List<Incident>;
        }
        set
        {
            base.SetValue(IncidentReportListProperty, value);
        }
    }

    public Incident Incident
    {
        get
        {
            return GetValue(IncidentProperty) as Incident;
        }
        set
        {
            SetValue(IncidentProperty, value);
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return (bool)GetValue(IsReadOnlyProperty);
        }
        set
        {
            SetValue(IsReadOnlyProperty, value);
        }
    }

    protected async Task OpenByIncident(Incident incident)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            {nameof(Domain.Incident), incident },
            {nameof(BaseVisitId), BaseVisitId },
            {nameof(ServiceUserId), ServiceUserId },
            {nameof(IsReadOnly), IsReadOnly }
        };

        var IncidentReportDetailpage = $"{nameof(IncidentReportPage)}?";
        await Shell.Current.GoToAsync(IncidentReportDetailpage, navigationParameter);
    }
}