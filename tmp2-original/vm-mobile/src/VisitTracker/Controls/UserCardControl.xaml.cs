namespace VisitTracker;

public partial class UserCardControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> CallCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DirectionsCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OpenProfilePictureCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OpenUserDetailCommand { get; set; }

    public UserCardControl()
    {
        InitializeComponent();

        CallCommand = ReactiveCommand.CreateFromTask(Call);
        DirectionsCommand = ReactiveCommand.CreateFromTask(Directions);
        OpenProfilePictureCommand = ReactiveCommand.CreateFromTask(OpenProfilePicture);
        OpenUserDetailCommand = ReactiveCommand.CreateFromTask(OpenUserDetail);

        BindBusy(CallCommand);
        BindBusy(DirectionsCommand);
        BindBusy(OpenProfilePictureCommand);
        BindBusy(OpenUserDetailCommand);
    }

    protected async Task Call()
    {
        await SystemHelper.Current.Open(User?.Phone);
    }

    protected async Task OpenUserDetail()
    {
        await TapAnimationAsync(UserCardContainer);

        var detailPage = User.UserType switch
        {
            EUserType.SERVICEUSER or EUserType.NEXTOFKIN => $"{nameof(ServiceUserDetailPage)}",
            EUserType.CAREWORKER => $"{nameof(CareWorkerDetailPage)}",
            _ => string.Empty
        };

        if (detailPage == string.Empty) return;

        if (Shell.Current.Navigation.NavigationStack != null
            && Shell.Current.Navigation.NavigationStack.Any(x =>
                x != null && x.GetType().Name == detailPage))
            return;

        await Shell.Current.GoToAsync($"{detailPage}?Id={User.UserId}");
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

    protected async Task OpenProfilePicture()
    {
        if (string.IsNullOrEmpty(User?.ImageUrl))
        {
            await App.Current.MainPage.ShowSnackbar(Messages.ProfilePictureNotAvailable, false, true);
            return;
        }

        await Launcher.Default.OpenAsync(new OpenFileRequest(User.Name, new ReadOnlyFile(User.ImageUrl)));
    }

    protected async Task Directions()
    {
        if (!string.IsNullOrEmpty(User?.UserAddressCard?.Address) &&
            !string.IsNullOrEmpty(User?.UserAddressCard?.Latitude) &&
            !string.IsNullOrEmpty(User?.UserAddressCard?.Longitude))
        {
            await Map.OpenAsync(
                new Location(Convert.ToDouble(User?.UserAddressCard?.Latitude),
                    Convert.ToDouble(User?.UserAddressCard?.Longitude)),
                    new MapLaunchOptions()
                    {
                        Name = User?.UserAddressCard?.Address,
                        NavigationMode = string.IsNullOrEmpty(User?.TransportationMode) ? NavigationMode.Default :
                        Enum.TryParse(User?.TransportationMode, true, out NavigationMode mode) ? mode : NavigationMode.Default
                    }
                );
    }
    }

    public static readonly BindableProperty UserProperty = BindableProperty.Create(nameof(User),
            typeof(UserCardDto), typeof(UserCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: UserPropertyChanged);

    public static readonly BindableProperty IsCallVisibleProperty = BindableProperty.Create(nameof(IsCallVisible),
            typeof(bool), typeof(UserCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: IsCallVisiblePropertyChanged);

    public static readonly BindableProperty IsDirectionVisibleProperty = BindableProperty.Create(nameof(IsDirectionVisible),
            typeof(bool), typeof(UserCardControl), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: IsDirectionVisiblePropertyChanged);

    public static readonly BindableProperty IsAccessInstructionsVisibleProperty = BindableProperty.Create(nameof(IsAccessInstructionsVisible),
            typeof(bool), typeof(UserCardControl), false, defaultBindingMode: BindingMode.OneWay,
            propertyChanged: IsAccessInstructionsVisiblePropertyChanged);

    public static readonly BindableProperty IsUserNavigationVisibleProperty = BindableProperty.Create(nameof(IsUserNavigationVisible),
            typeof(bool), typeof(UserCardControl), true, defaultBindingMode: BindingMode.OneWay,
            propertyChanged: IsUserNavigationVisiblePropertyChanged);

    public static readonly BindableProperty HasCardTitleProperty = BindableProperty.Create(nameof(HasCardTitle),
        typeof(bool), typeof(UserCardControl),
        defaultValue: true, defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: HasCardTitlePropertyChanged);

    private static void UserPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (UserCardControl)bindable;
        control.User = newValue as UserCardDto;

        if (control.User != null && control.User.UserType != EUserType.SERVICEUSER)
        {
            control.IsAccessInstructionsVisible = false;
            control.IsDirectionVisible = false;
            if (control.User.UserAddressCard != null)
                control.User.UserAddressCard.Address = null;
        }
    }

    private static void HasCardTitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (UserCardControl)bindable;
        control.HasCardTitle = (bool)newValue;
    }

    private static void IsCallVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (UserCardControl)bindable;
        control.IsCallVisible = (bool)newValue;
    }

    private static void IsDirectionVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (UserCardControl)bindable;
        control.IsDirectionVisible = (bool)newValue;
    }

    private static void IsAccessInstructionsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (UserCardControl)bindable;
        control.IsAccessInstructionsVisible = (bool)newValue;
    }

    private static void IsUserNavigationVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (UserCardControl)bindable;
        control.IsUserNavigationVisible = (bool)newValue;
    }

    public bool IsCallVisible
    {
        get
        {
            return (bool)GetValue(IsCallVisibleProperty);
        }
        set
        {
            SetValue(IsCallVisibleProperty, value);
        }
    }

    public bool IsDirectionVisible
    {
        get
        {
            return (bool)GetValue(IsDirectionVisibleProperty);
        }
        set
        {
            SetValue(IsDirectionVisibleProperty, value);
        }
    }

    public bool IsAccessInstructionsVisible
    {
        get
        {
            return (bool)GetValue(IsAccessInstructionsVisibleProperty);
        }
        set
        {
            SetValue(IsAccessInstructionsVisibleProperty, value);
        }
    }

    public bool IsUserNavigationVisible
    {
        get
        {
            return (bool)GetValue(IsUserNavigationVisibleProperty);
        }
        set
        {
            SetValue(IsUserNavigationVisibleProperty, value);
        }
    }

    public UserCardDto User
    {
        get
        {
            return GetValue(UserProperty) as UserCardDto;
        }
        set
        {
            SetValue(UserProperty, value);
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