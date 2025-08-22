namespace VisitTracker;

public partial class BookingCardControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenProfilePictureCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OpenBookingDetailCommand { get; set; }
    public ReactiveCommand<int, Unit> OpenVisitCommand { get; set; }

    public BookingCardControl()
    {
        InitializeComponent();

        OpenProfilePictureCommand = ReactiveCommand.CreateFromTask(OpenProfilePicture);
        OpenBookingDetailCommand = ReactiveCommand.CreateFromTask(OpenBookingDetail);
        OpenVisitCommand = ReactiveCommand.CreateFromTask<int>(OpenVisit);

        BindBusy(OpenProfilePictureCommand);
        BindBusy(OpenBookingDetailCommand);
        BindBusy(OpenVisitCommand);
    }

    protected async Task OpenBookingDetail()
    {
        await TapAnimationAsync(BookingCardVerticalLayout);

        if (Visits == null || !Visits.Any() || !IsVisibleReport)
        {
            var navigationParameter = new Dictionary<string, object> { { "Id", Booking.Id } };
            var bookingDetailPage = $"{nameof(BookingDetailPage)}?";

            await Shell.Current.GoToAsync(bookingDetailPage, navigationParameter);
        }

    }

    protected async Task OpenVisit(int id)
    {
        var navigationParameter = new Dictionary<string, object> {
            { "Id", Booking.Id },
            { "VisitId", id }
        };
        var bookingDetailPage = $"{nameof(BookingDetailPage)}?";

        await Shell.Current.GoToAsync(bookingDetailPage, navigationParameter);
    }
    private async Task TapAnimationAsync(VisualElement element)
    {
        if (element == null)
            return;

        element.AnchorX = 0.5;
        element.AnchorY = 0.5;

        await element.ScaleTo(0.975, 50); 
        await element.ScaleTo(1.0, 50);
    }

    private void OnImageTapped(object sender, TappedEventArgs e)
    {
        string imageUrl = e.Parameter as string;
        if (!string.IsNullOrEmpty(imageUrl))
        {
            var popup = new ImagePopup(imageUrl);
            Application.Current.MainPage.ShowPopup(popup);
        }
    }

    protected async Task OpenProfilePicture()
    {
        if (string.IsNullOrEmpty(ServiceUser?.ImageUrl))
        {
            await App.Current.MainPage.ShowSnackbar(Messages.ProfilePictureNotAvailable, false, true);
            return;
        }

        await Launcher.Default.OpenAsync(new OpenFileRequest(ServiceUser.Name, new ReadOnlyFile(ServiceUser.ImageUrl)));
    }

    public static readonly BindableProperty IsNavigationVisibleProperty = BindableProperty.Create(nameof(IsNavigationVisible),
            typeof(bool), typeof(BookingCardControl), true, defaultBindingMode: BindingMode.OneWay,
            propertyChanged: IsNavigationVisiblePropertyChanged);

    public static readonly BindableProperty BookingProperty = BindableProperty.Create(nameof(Booking),
            typeof(Booking), typeof(BookingCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: BookingPropertyChanged);

    public static readonly BindableProperty ServiceUserProperty = BindableProperty.Create(nameof(ServiceUser),
            typeof(ServiceUser), typeof(BookingCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: ServiceUserPropertyChanged);

    public static readonly BindableProperty PrimaryCareWorkerProperty = BindableProperty.Create(nameof(PrimaryCareWorker),
            typeof(CareWorker), typeof(BookingCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: PrimaryCareWorkerPropertyChanged);

    public static readonly BindableProperty SecondaryCareWorkerProperty = BindableProperty.Create(nameof(SecondaryCareWorker),
            typeof(CareWorker), typeof(BookingCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: SecondaryCareWorkerPropertyChanged);

    public static readonly BindableProperty PrimaryCareWorkerEtaProperty = BindableProperty.Create(nameof(PrimaryCareWorkerEta),
            typeof(EtaMobileDto), typeof(BookingCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: PrimaryCareWorkerEtaPropertyChanged);

    public static readonly BindableProperty SecondaryCareWorkerEtaProperty = BindableProperty.Create(nameof(SecondaryCareWorkerEta),
            typeof(EtaMobileDto), typeof(BookingCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: SecondaryCareWorkerEtaPropertyChanged);

    public static readonly BindableProperty VisitsProperty = BindableProperty.Create(nameof(Visits),
            typeof(List<Visit>), typeof(BookingCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: VisitsPropertyChanged);

    public static readonly BindableProperty IsVisibleReportProperty = BindableProperty.Create(nameof(IsVisibleReport),
            typeof(bool), typeof(BookingCardControl), true, defaultBindingMode: BindingMode.OneWay,
            propertyChanged: IsVisibleReportPropertyChanged);

    private static void IsVisibleReportPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.IsVisibleReport = (bool)newValue;
    }

    private static void IsNavigationVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.IsNavigationVisible = (bool)newValue;
    }

    private static void BookingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.Booking = newValue as Booking;
    }

    private static void ServiceUserPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.ServiceUser = newValue as ServiceUser;
    }

    private static void PrimaryCareWorkerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.PrimaryCareWorker = newValue as CareWorker;
    }

    private static void SecondaryCareWorkerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.SecondaryCareWorker = newValue as CareWorker;
    }

    private static void PrimaryCareWorkerEtaPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.PrimaryCareWorkerEta = newValue as EtaMobileDto;
    }

    private static void SecondaryCareWorkerEtaPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.SecondaryCareWorkerEta = newValue as EtaMobileDto;
    }

    private static void VisitsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BookingCardControl)bindable;
        control.Visits = newValue as List<Visit>;
    }

    public bool IsVisibleReport
    {
        get
        {
            return (bool)GetValue(IsVisibleReportProperty);
        }
        set
        {
            SetValue(IsVisibleReportProperty, value);
        }
    }

    public bool IsNavigationVisible
    {
        get
        {
            return (bool)GetValue(IsNavigationVisibleProperty);
        }
        set
        {
            SetValue(IsNavigationVisibleProperty, value);
        }
    }

    public Booking Booking
    {
        get
        {
            return GetValue(BookingProperty) as Booking;
        }
        set
        {
            SetValue(BookingProperty, value);
        }
    }

    public ServiceUser ServiceUser
    {
        get
        {
            return GetValue(ServiceUserProperty) as ServiceUser;
        }
        set
        {
            SetValue(ServiceUserProperty, value);
        }
    }

    public CareWorker PrimaryCareWorker
    {
        get
        {
            return GetValue(PrimaryCareWorkerProperty) as CareWorker;
        }
        set
        {
            SetValue(PrimaryCareWorkerProperty, value);
        }
    }

    public CareWorker SecondaryCareWorker
    {
        get
        {
            return GetValue(SecondaryCareWorkerProperty) as CareWorker;
        }
        set
        {
            SetValue(SecondaryCareWorkerProperty, value);
        }
    }

    public EtaMobileDto PrimaryCareWorkerEta
    {
        get
        {
            return GetValue(PrimaryCareWorkerEtaProperty) as EtaMobileDto;
        }
        set
        {
            SetValue(PrimaryCareWorkerEtaProperty, value);
        }
    }

    public EtaMobileDto SecondaryCareWorkerEta
    {
        get
        {
            return GetValue(SecondaryCareWorkerEtaProperty) as EtaMobileDto;
        }
        set
        {
            SetValue(SecondaryCareWorkerEtaProperty, value);
        }
    }

    public List<Visit> Visits
    {
        get
        {
            return GetValue(VisitsProperty) as List<Visit>;
        }
        set
        {
            SetValue(VisitsProperty, value);
        }
    }
}