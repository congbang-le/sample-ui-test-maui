namespace VisitTracker;

public class CareWorkersVm : BaseVm
{
    public IList<CareWorker> AllCareWorkers { get; set; }
    public IList<CareWorker> FilteredCareWorkers { get; set; }
    public bool IsRefreshing { get; set; }

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public ReactiveCommand<int, Unit> OpenCareWorkerCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<string, Unit> CallCommand { get; }
    public ReactiveCommand<string, Unit> SearchTextChangedCommand { get; }

    public CareWorkersVm()
    {
        OpenCareWorkerCommand = ReactiveCommand.CreateFromTask<int>(OpenCareWorker);
        RefreshCommand = ReactiveCommand.CreateFromTask(OnRefresh);
        CallCommand = ReactiveCommand.CreateFromTask<string>(OnCall);
        SearchTextChangedCommand = ReactiveCommand.CreateFromTask<string>(SearchTextChanged);

        this.WhenAnyValue(vm => vm.SearchText)
            .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
            .DistinctUntilChanged()
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(SearchTextChangedCommand);

        BindBusyWithException(OpenCareWorkerCommand);
        BindBusyWithException(RefreshCommand);
        BindBusyWithException(CallCommand);
        BindBusyWithException(SearchTextChangedCommand);
    }

    protected override async Task Init()
    {
        var careWorkers = await AppServices.Current.CareWorkerService.GetAll();
        FilteredCareWorkers = AllCareWorkers = careWorkers.OrderBy(cw => cw.Name).ToList();
    }

    protected async Task OpenCareWorker(int CareWorkerId)
    {
        await Shell.Current.Navigation.PopToRootAsync();

        var careWorkerDetailPage = $"{nameof(CareWorkerDetailPage)}?Id={CareWorkerId}";
        await Shell.Current.GoToAsync(careWorkerDetailPage);
    }

    protected async Task SearchTextChanged(string searchText)
    {
        if (AllCareWorkers == null) return;

        string searchLower = SearchText?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(searchLower))
            FilteredCareWorkers = AllCareWorkers.ToList();
        else
        {
            FilteredCareWorkers = AllCareWorkers
                .Where(x => x.Name != null && x.Name.ToLowerInvariant().Contains(searchLower))
                .ToList();
        }

        FilteredCareWorkers = FilteredCareWorkers.OrderBy(cw => cw.Name).ToList();
    }

    protected async Task OnRefresh()
    {
        await AppServices.Current.SupervisorService.SyncData();

        await InitCommand.Execute(true);
        IsRefreshing = false;
    }

    protected async Task OnCall(string phone)
    {
        await SystemHelper.Current.Open(phone);
    }
}