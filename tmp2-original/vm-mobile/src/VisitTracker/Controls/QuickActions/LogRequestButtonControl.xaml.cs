namespace VisitTracker;

public partial class LogRequestButtonControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenLogRequestCommand { get; set; }

    public LogRequestButtonControl()
    {
        InitializeComponent();

        OpenLogRequestCommand = ReactiveCommand.CreateFromTask(OpenLogRequest);
        BindBusy(OpenLogRequestCommand);
    }

    public int BookingId
    {
        get
        {
            return Convert.ToInt32(GetValue(BookingIdProperty));
        }
        set
        {
            SetValue(BookingIdProperty, value);
        }
    }

    public static readonly BindableProperty BookingIdProperty = BindableProperty.Create(nameof(BookingId),
     typeof(int), typeof(LogRequestButtonControl), defaultBindingMode: BindingMode.TwoWay,
     propertyChanged: BookingIdPropertyChanged);

    private static void BookingIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (LogRequestButtonControl)bindable;
        control.BookingId = Convert.ToInt32(newValue);
    }

    protected async Task OpenLogRequest()
    {
        var url = AppData.Current.ExternalLinks.FirstOrDefault(x => x.LinkType == EExternalLinkType.LOG_REQUEST)?.ServerUrl;
        await OpenActions.OpenMiscDetailPage(url, "Log Request");
    }
}