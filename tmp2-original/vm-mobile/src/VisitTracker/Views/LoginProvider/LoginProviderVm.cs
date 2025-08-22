namespace VisitTracker;

public class LoginProviderVm : BaseVm
{
    [Reactive] public Provider Provider { get; set; }
    [Reactive] public string AccessCode { get; set; }

    public ReactiveCommand<Unit, Unit> ForgotCodeCommand { get; }
    public ReactiveCommand<bool, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }
    public ReactiveCommand<Unit, Unit> ReturnCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenQrCommand { get; }
    public double PanelHeight { get; set; } = 0.0;
    public LoginProviderVm()
    {
        ForgotCodeCommand = ReactiveCommand.CreateFromTask(ForgotCode);
        LoginCommand = ReactiveCommand.CreateFromTask<bool>(Login);
        ConfirmCommand = ReactiveCommand.CreateFromTask(Confirm);
        ReturnCommand = ReactiveCommand.CreateFromTask(Return);
        OpenQrCommand = ReactiveCommand.CreateFromTask(OpenQr);

        BindBusyWithException(ForgotCodeCommand);
        BindBusyWithException(LoginCommand);
        BindBusyWithException(ConfirmCommand);
        BindBusyWithException(ReturnCommand);
        BindBusyWithException(OpenQrCommand);
    }

    protected override async Task Init()
    {
        var isRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.QrProviderCodeMessage>(this);
        if (!isRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.QrProviderCodeMessage>(this,
                async (recipient, message) =>
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        AccessCode = message.Value;
                        await Login();
                    });
                });
        PanelHeight = SystemHelper.Current.SetCardViewHeight(50,true) -80 ;
    }

    protected async Task Login(bool isSuccess = false)
    {
        SystemHelper.Current.HideKeyboard();

        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.NoInternet, false);
            return;
        }

        var currentTime = DateTimeExtensions.NowNoTimezone();
        var loginCount = await SecureStorage.Default.GetAsync(Constants.KeyInvLoginProvCount);
        var lastLogin = await SecureStorage.Default.GetAsync(Constants.KeyInvLoginProvTime);

        if (loginCount == null || Convert.ToInt32(loginCount) <= Constants.ProviderLoginValidAttempts ||
            ((currentTime - Convert.ToDateTime(lastLogin)).TotalMinutes > Constants.ProviderLoginBlockedInMins))
        {
            Provider = await AppServices.Current.ProviderService.LoginProvider(AccessCode);
            if (Provider != null)
            {
                SecureStorage.Default.Remove(Constants.KeyInvLoginCount);
                SecureStorage.Default.Remove(Constants.KeyInvLoginTime);
                await SecureStorage.Default.SetAsync(Constants.KeyAppLocation, Provider.ServerUrl);
            }
            else
            {
                await SecureStorage.Default.SetAsync(Constants.KeyInvLoginProvCount, (!string.IsNullOrEmpty(loginCount) &&
                    (currentTime - Convert.ToDateTime(lastLogin)).TotalMinutes < Constants.ProviderLoginBlockedInMins) ?
                    (Convert.ToInt32(loginCount) + 1).ToString() : 1.ToString());
                await SecureStorage.Default.SetAsync(Constants.KeyInvLoginProvTime, currentTime.ToString());
                await Application.Current.MainPage.ShowSnackbar(Messages.ProviderCodeInvalid, false);
            }
        }
        else await Application.Current.MainPage.ShowSnackbar(Messages.ProviderMutipleAttempts, true);
    }

    protected async Task Confirm()
    {
        await Task.Delay(200);
        await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage = ServiceLocator.GetService<LoginPage>());
    }

    protected async Task Return()
    {
        await AppServices.Current.ProviderService.DeleteAll();
        SecureStorage.Default.Remove(Constants.KeyAppLocation);
        Provider = null;
    }

    protected async Task OpenQr() =>
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(ServiceLocator.GetService<LoginProviderQrScannerPage>()));

    protected async Task ForgotCode() => await Application.Current.MainPage.ShowSnackbar(Messages.LoginForgotProviderCode, false, true);

}