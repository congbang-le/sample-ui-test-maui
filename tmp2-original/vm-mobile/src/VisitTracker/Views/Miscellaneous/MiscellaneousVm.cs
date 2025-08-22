namespace VisitTracker;

public class MiscellaneousVm : BaseVm
{
    public string ProfileName { get; set; }
    public string ProfileImageUrl { get; set; }

    public List<ExternalLinkDto> ExternalLinks { get; set; }

    public List<MiscActionVm> Actions { get; set; }
    public ReactiveCommand<ExternalLinkDto, Unit> OpenMiscDetailPageCommand { get; }

    public ReactiveCommand<Unit, Unit> EmergencyCommand { get; }
    public ReactiveCommand<Unit, Unit> CallCommand { get; }
    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenMarChartCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenFluidChartCommand { get; }

    public MiscellaneousVm()
    {
        LogoutCommand = ReactiveCommand.CreateFromTask(Logout);
        OpenMiscDetailPageCommand = ReactiveCommand.CreateFromTask<ExternalLinkDto>(OpenMiscDetailPage);
        EmergencyCommand = ReactiveCommand.CreateFromTask(Emergency);
        CallCommand = ReactiveCommand.CreateFromTask(Call);
        OpenMarChartCommand = ReactiveCommand.CreateFromTask(OpenMarChart);
        OpenFluidChartCommand = ReactiveCommand.CreateFromTask(OpenFluidChart);

        BindBusyWithException(LogoutCommand);
        BindBusyWithException(OpenMiscDetailPageCommand);
        BindBusyWithException(EmergencyCommand);
        BindBusyWithException(CallCommand);
        BindBusyWithException(OpenMarChartCommand);
        BindBusyWithException(OpenFluidChartCommand);
    }

    protected override async Task Init()
    {
        ProfileName = AppData.Current.CurrentProfile.Name;
        ProfileImageUrl = AppData.Current.CurrentProfile.ImageUrl;

        ExternalLinks = AppData.Current.ExternalLinks.ToList();

        Actions = new List<MiscActionVm>
        {
            new MiscActionVm
            {
                Command = EmergencyCommand,
                Icon = MaterialCommunityIconsFont.AlertOctagon,
                Text = "Emergency",
                Order = 1
            },
            new MiscActionVm
            {
                Command = CallCommand,
                Icon = MaterialCommunityIconsFont.Phone,
                Text = "Office",
                 Order = 2
            },
            new MiscActionVm
            {
                Command = LogoutCommand,
                Icon = MaterialCommunityIconsFont.Logout,
                Text = "Logout",
                Order = 5
            },
        };

        if (AppData.Current.CurrentProfile?.Type == nameof(EUserType.SERVICEUSER) ||
            AppData.Current.CurrentProfile?.Type == nameof(EUserType.NEXTOFKIN))
        {
            Actions.Add(new MiscActionVm
            {
                Command = OpenMarChartCommand,
                Icon = MaterialCommunityIconsFont.ChartTimeline,
                Text = "MAR Chart",
                Order = 3
            });
            Actions.Add(new MiscActionVm
            {
                Command = OpenFluidChartCommand,
                Icon = MaterialCommunityIconsFont.Water,
                Text = "Fluid Chart",
                Order = 4
            });
        }

        Actions = Actions.OrderBy(x => x.Order).ToList();
        await Task.CompletedTask;
    }

    protected async Task OpenMiscDetailPage(ExternalLinkDto misc)
    {
        await OpenActions.OpenMiscDetailPage(misc.ServerUrl, misc.Title);
    }

    protected async Task Logout()
    {
        if (App.IsTracking || App.IsFingerprinting)
        {
            await App.Current.MainPage.DisplayAlert(Messages.Error, Messages.LogoutFailedTracking, Messages.Ok);
            return;
        }

        bool doLogout = false;
        var syncDataCount = await AppServices.Current.SyncService.GetAll();
        if (syncDataCount != null && syncDataCount.Any())
        {
            await AppServices.Current.SyncService.SyncData();
            await AppServices.Current.SyncService.DeleteAllBySyncData();

            syncDataCount = await AppServices.Current.SyncService.GetAll();
            if (syncDataCount != null && syncDataCount.Any())
            {
                doLogout = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle, Messages.LogoutFailedPendingUploadConfirm, Messages.Yes, Messages.No);
                if (!doLogout)
                {
                    await AppServices.Current.BackgroundService.StartBackgroundTimer(3, 30);
                    return;
                }
            }
        }
        else doLogout = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle, Messages.DialogLogoutConfirmation, Messages.Yes, Messages.No);

        if (doLogout) await SystemHelper.Current.Logout();
    }

    protected async Task Call()
    {
        var provider = await AppServices.Current.ProviderService.GetLoggedInProvider();
        await SystemHelper.Current.Open(provider?.Phone);
    }

    protected async Task Emergency()
    {
        string action = await App.Current.MainPage.DisplayActionSheet(Messages.Emergency, Messages.Cancel, null, Messages.Ambulance, Messages.Fire);
        switch (action)
        {
            case "Ambulance":
                await SystemHelper.Current.Open("999");
                break;

            case "Fire":
                await SystemHelper.Current.Open("999");
                break;
        }
    }

    protected async Task OpenMarChart()
    {
        var marChartPage = $"{nameof(MarChartPage)}?Id={AppData.Current.CurrentProfile?.Id}";
        await Shell.Current.GoToAsync(marChartPage);
    }

    protected async Task OpenFluidChart()
    {
        var fluidChartPage = $"{nameof(FluidChartPage)}?Id={AppData.Current.CurrentProfile?.Id}";
        await Shell.Current.GoToAsync(fluidChartPage);
    }
}

public class MiscActionVm
{
    public ReactiveCommand<Unit, Unit> Command { get; set; }
    public string Icon { get; set; }
    public string Text { get; set; }
    public short Order { get; set; }
}