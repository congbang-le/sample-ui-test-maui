namespace VisitTracker;

public class SupervisorDashboardVm : BaseVm
{
    public SupervisorDashboardDto DashboardDto { get; set; }
    public ReactiveCommand<int, Unit> ServiceUserTapCommand { get; }
    public ReactiveCommand<NotificationsDto, Unit> FormsRefreshCommand { get; }
    public ReactiveCommand<object, Unit> TabChangeCommand { get; }
    public bool IsAllServiceUsersFingerprinted { get; set; } = false;
    public IList<ServiceUser> FilteredServiceUsers { get; set; }
    public bool IsAllServiceUsersHasGroundTruths { get; set; } = false;
    private bool isInitialized;

    public SupervisorDashboardVm()
    {
        ServiceUserTapCommand = ReactiveCommand.CreateFromTask<int>(ServiceUserTapped);
        FormsRefreshCommand = ReactiveCommand.CreateFromTask<NotificationsDto>(FormsRefresh);
        TabChangeCommand = ReactiveCommand.CreateFromTask<object>(OnTabChange);

        BindBusyWithException(ServiceUserTapCommand);
        BindBusyWithException(FormsRefreshCommand);
    }

    protected override async Task Init()
    {
        if (DashboardDto == null)
        {
            DashboardDto = new SupervisorDashboardDto();
            DashboardDto.MissingFingerprints = new List<MissingFingerprintDto>();
            DashboardDto.MissingGroundTruths = new List<ServiceUser>();
        }

        DashboardDto.ProfileName = AppData.Current.CurrentProfile.Name;
        DashboardDto.MissingFingerprints = await GetMissingFingerprintsAsync();
        DashboardDto.MissingGroundTruths = await GetMissingGroundTruths();

        await FormsRefreshCommand.Execute();
        WeakReferenceMessenger.Default.Send(new MessagingEvents.DataRetentionMessage(true));
        SetCardViewHeight("FP");
        isInitialized = true;
    }

    private async Task<List<ServiceUser>> GetMissingGroundTruths() {
        var MissingGroundTruths = (await AppServices.Current.ServiceUserService.GetAllByMissingGroundTruths()).OrderBy(a => a.Name).ToList();
        IsAllServiceUsersHasGroundTruths = MissingGroundTruths == null || !MissingGroundTruths.Any();
        return MissingGroundTruths;
    }
    private async Task<List<MissingFingerprintDto>> GetMissingFingerprintsAsync()
    {
        var allServiceUsers = await AppServices.Current.ServiceUserService.GetAll();
        var usersWithoutAnyFp = allServiceUsers
            .Where(su => su.iOSFpAvailable != true || su.AndroidFpAvailable != true)
            .ToList();

        var missingFingerprintsDto = new List<MissingFingerprintDto>();

        foreach (var su in usersWithoutAnyFp)
        {
            missingFingerprintsDto.Add(new MissingFingerprintDto
            {
                ServiceUser = su,
                IsAndroidMissing = su.AndroidFpAvailable == true
                                    ? "False"
                                    : su.AndroidFpAvailable == null
                                        ? "Null"
                                        : "True",

                IsiOSMissing = su.iOSFpAvailable == true
                                    ? "False"
                                    : su.iOSFpAvailable == null
                                        ? "Null"
                                        : "True"
            });
        }
        IsAllServiceUsersFingerprinted = missingFingerprintsDto.Count == 0;
        return missingFingerprintsDto.ToList();
    }

    private async Task OnTabChange(object parameter)
    {
        if (!isInitialized) return;
        // TODO: Avoid calling the API when changing tabs if the data is already loaded.
        if (parameter is UraniumUI.Material.Controls.TabItem tabItem)
        {
            var newTabTitle = tabItem.Title;

            if (newTabTitle == "Fingerprint")
            {
                DashboardDto.MissingFingerprints = await GetMissingFingerprintsAsync();
                SetCardViewHeight("FP");
            }
            else if (newTabTitle == "Ground Truth")
            {
                DashboardDto.MissingGroundTruths = await GetMissingGroundTruths();
                SetCardViewHeight("GT");
            }
        }
    }

    protected async Task ServiceUserTapped(int Id)
    {
        var bookingDetailPage = $"{nameof(ServiceUserDetailPage)}?Id={Id}";
        await Shell.Current.GoToAsync(bookingDetailPage);
    }

    private void SetCardViewHeight(string selectedtab)
    {
        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        var cardHeight = screenHeight - 550;

        int dataHeight = selectedtab switch
        {
            "FP" => DashboardDto.MissingFingerprints.Count * 66,
            _ => DashboardDto.MissingGroundTruths.Count * 64
        };
        DashboardDto.CardViewHeight = Math.Min(dataHeight, cardHeight);
    }

    public async Task FormsRefresh(NotificationsDto notification)
    {
        var response = await AppServices.Current.SupervisorService.GetFormDetailsBySup(AppData.Current.CurrentProfile.Id);
        DashboardDto.PendingFormsCount = response?.PendingFormsCount ?? default;
        DashboardDto.ScheduledFormsCount = response?.ScheduledFormsCount ?? default;
        DashboardDto.SubmittedFormsCount = response?.SubmittedFormsCount ?? default;
        DashboardDto.OverdueFormsCount = response?.OverdueFormsCount ?? default;
    }
}