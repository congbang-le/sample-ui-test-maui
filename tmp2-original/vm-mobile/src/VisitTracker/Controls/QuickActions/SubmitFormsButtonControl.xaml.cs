namespace VisitTracker;

public partial class SubmitFormsButtonControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenSubmitFormsCommand { get; set; }

    public SubmitFormsButtonControl()
    {
        InitializeComponent();

        OpenSubmitFormsCommand = ReactiveCommand.CreateFromTask(OpenSubmitForms);
        BindBusy(OpenSubmitFormsCommand);
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
     typeof(int), typeof(SubmitFormsButtonControl), defaultBindingMode: BindingMode.TwoWay,
     propertyChanged: BookingIdPropertyChanged);

    private static void BookingIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SubmitFormsButtonControl)bindable;
        control.BookingId = Convert.ToInt32(newValue);
    }

    protected async Task OpenSubmitForms()
    {
        var url = AppData.Current.ExternalLinks.FirstOrDefault(x => x.LinkType == EExternalLinkType.SUBMIT_FORMS)?.ServerUrl;
        await OpenActions.OpenMiscDetailPage(url, "Submit Forms");
    }
}