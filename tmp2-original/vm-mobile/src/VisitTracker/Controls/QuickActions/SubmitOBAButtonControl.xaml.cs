namespace VisitTracker;

public partial class SubmitOBAFormsButtonControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenSubmitOBAFormsCommand { get; set; }

    public SubmitOBAFormsButtonControl()
    {
        InitializeComponent();

        OpenSubmitOBAFormsCommand = ReactiveCommand.CreateFromTask(OpenSubmitOBAForms);
        BindBusy(OpenSubmitOBAFormsCommand);
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
     typeof(int), typeof(SubmitOBAFormsButtonControl), defaultBindingMode: BindingMode.TwoWay,
     propertyChanged: BookingIdPropertyChanged);

    private static void BookingIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SubmitOBAFormsButtonControl)bindable;
        control.BookingId = Convert.ToInt32(newValue);
    }

    protected async Task OpenSubmitOBAForms()
    {
        var url = SystemHelper.Current.GetUrl(Constants.TpUrlOBAForms, true) + "/" + AppData.Current.CurrentProfile.UserName;
        await OpenActions.OpenMiscDetailPage(url, "Submit OBA Forms");
    }
}