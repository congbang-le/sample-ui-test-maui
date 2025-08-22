namespace VisitTracker;

public class ServiceUsersVm : BaseVm, IDisposable
{
    public IList<ServiceUser> AllServiceUsers { get; set; }
    public IList<UserCardDto> ServiceUserCards { get; set; }

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public IList<ServiceUser> FilteredServiceUsers { get; set; }

    public bool IsRefreshing { get; set; }

    public float FpProgress { get; set; }
    public UserCardDto FpServiceUserCard { get; set; }

    public bool IsVisibleNoDataControl { get; set; }

    public UserCardDto FormsServiceUserCard { get; set; }

    public ReactiveCommand<int, Unit> OpenServiceUserCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<string, Unit> SearchTextChangedCommand { get; }

    public ServiceUsersVm()
    {
        OpenServiceUserCommand = ReactiveCommand.CreateFromTask<int>(OpenServiceUser);
        RefreshCommand = ReactiveCommand.CreateFromTask(OnRefresh);
        SearchTextChangedCommand = ReactiveCommand.CreateFromTask<string>(SearchTextChanged);

        this.WhenAnyValue(vm => vm.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
            .DistinctUntilChanged()
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(SearchTextChangedCommand);

        BindBusyWithException(OpenServiceUserCommand);
        BindBusyWithException(RefreshCommand);
        BindBusyWithException(SearchTextChangedCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        FilteredServiceUsers = AllServiceUsers = await AppServices.Current.ServiceUserService.GetAll();
        var userCards = await BaseAssembler.BuildUserCardsFromSu(AllServiceUsers);

        ServiceUserCards = userCards.OrderBy(u => u.Name).ToList();

        var fpServiceUserId = Preferences.Default.Get<int>(Constants.PrefKeyOngoingFpServiceUserId, default);
        if (fpServiceUserId != default)
        {
            FpServiceUserCard = ServiceUserCards.FirstOrDefault(x => x.UserId == fpServiceUserId);
            ServiceUserCards = ServiceUserCards.Where(x => x.UserId != fpServiceUserId).ToList();
        }
        else FpServiceUserCard = null;

        FormsServiceUserCard = null;
        IsVisibleNoDataControl = true;
    }

    protected async Task OpenServiceUser(int serviceUserId)
    {
        await Shell.Current.Navigation.PopToRootAsync();

        var serviceUserDetailPage = $"{nameof(ServiceUserDetailPage)}?Id={serviceUserId}";
        await Shell.Current.GoToAsync(serviceUserDetailPage);
    }

    protected async Task OnRefresh()
    {
        await AppServices.Current.SupervisorService.SyncData();

        await InitCommand.Execute(true);
        IsRefreshing = false;
    }

    protected async Task SearchTextChanged(string searchText)
    {
        if (AllServiceUsers == null) return;

        string searchLower = SearchText?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(searchLower))
            FilteredServiceUsers = AllServiceUsers.ToList();
        else
        {
            FilteredServiceUsers = AllServiceUsers
                .Where(x => x.Name != null && x.Name.ToLowerInvariant().Contains(searchLower))
                .ToList();
        }

        var userCards = await BaseAssembler.BuildUserCardsFromSu(FilteredServiceUsers);
        ServiceUserCards = userCards.OrderBy(u => u.Name).ToList();
    }

    private void OnFingerprintStart(bool isSuccess)
    {
        IsVisibleNoDataControl = false;
        var fpServiceUserId = Preferences.Default.Get<int>(Constants.PrefKeyOngoingFpServiceUserId, default);
        if (fpServiceUserId != default)
        {
            FpServiceUserCard = ServiceUserCards.FirstOrDefault(x => x.UserId == fpServiceUserId);
            ServiceUserCards = ServiceUserCards.Where(x => x.UserId != fpServiceUserId).ToList();
        }
        else FpServiceUserCard = null;
    }

    private async Task OnFingerprintComplete(bool isSuccess)
    {
        await InitCommand.Execute(true);
        FpServiceUserCard = null;
        IsVisibleNoDataControl = true;
    }

    private async Task OnFingerprintProgress(float progress)
    {
        await Task.Run(() => FpProgress = progress);
    }

    private async Task OnVisitStart(bool isSuccess)
    {
        var supervisorVisitId = Preferences.Default.Get<int>(Constants.PrefKeyOngoingSupervisorVisitId, default);
        if (supervisorVisitId != default)
        {
            var supervisorVisit = await AppServices.Current.SupervisorVisitService.GetById(supervisorVisitId);
            if (supervisorVisit != null)
            {
                FormsServiceUserCard = ServiceUserCards.FirstOrDefault(x => x.UserId == supervisorVisit.ServiceUserId);
                ServiceUserCards = ServiceUserCards.Where(x => x.UserId != supervisorVisit.ServiceUserId).ToList();
            }
            else FormsServiceUserCard = null;
        }
    }

    private async Task OnVisitComplete(bool isSuccess)
    {
        await InitCommand.Execute(true);
        FormsServiceUserCard = null;
    }

    public void SubscribeAllMessagingCenters()
    {
        var isStartedFpRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.FingerprintStartedMessage>(this);
        if (!isStartedFpRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.FingerprintStartedMessage>(this,
                (recipient, message) => OnFingerprintStart(message.Value));

        var isCompletedFpRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.FingerprintCompletedMessage>(this);
        if (!isCompletedFpRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.FingerprintCompletedMessage>(this,
            async (recipient, message) => await OnFingerprintComplete(message.Value));

        var isProgressFpRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.FingerprintProgressMessage>(this);
        if (!isProgressFpRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.FingerprintProgressMessage>(this,
            async (recipient, message) => await OnFingerprintProgress(message.Value));

        var isStartedVisitRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.SupervisorVisitStartedMessage>(this);
        if (!isStartedVisitRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.SupervisorVisitStartedMessage>(this,
                async (recipient, message) => await OnVisitStart(message.Value));

        var isCompletedVisitRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.VisitCompletedMessage>(this);
        if (!isCompletedVisitRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.VisitCompletedMessage>(this,
                async (recipient, message) => await OnVisitComplete(message.Value));
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.FingerprintStartedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.FingerprintCompletedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.FingerprintProgressMessage>(this);

        WeakReferenceMessenger.Default.Unregister<MessagingEvents.SupervisorVisitStartedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.VisitCompletedMessage>(this);
    }
}