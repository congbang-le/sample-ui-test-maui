namespace VisitTracker;

public class LoginVm : BaseVm
{
    private CareWorkerHomePage _careWorkerHomePage { get; }
    private SupervisorHomePage _supervisorHomePage { get; }
    private ServiceUserHomePage _serviceUserHomePage { get; }
    private LoginProviderPage _loginProviderPage { get; }

    [Reactive] public Provider Provider { get; set; }
    [Reactive] public string Email { get; set; }
    [Reactive] public string Password { get; set; }

    public ReactiveCommand<bool, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> ForgotPasswordCommand { get; }
    public ReactiveCommand<Unit, Unit> OnBackCommand { get; }

    public LoginVm(CareWorkerHomePage careWorkerHomePage,
        SupervisorHomePage supervisorHomePage,
        ServiceUserHomePage serviceUserHomePage)
    {
        _careWorkerHomePage = careWorkerHomePage;
        _supervisorHomePage = supervisorHomePage;
        _serviceUserHomePage = serviceUserHomePage;

        LoginCommand = ReactiveCommand.CreateFromTask<bool>(Login);
        ForgotPasswordCommand = ReactiveCommand.CreateFromTask(ForgotPassword);
        OnBackCommand = ReactiveCommand.CreateFromTask(OnBack);

        BindBusyWithException(LoginCommand);
        BindInputDisabledWhileExecuting(LoginCommand, Observable.Return(true));
        BindBusyWithException(ForgotPasswordCommand);
        BindBusyWithException(OnBackCommand);
    }

    protected override async Task Init()
    {
        Provider = await AppServices.Current.ProviderService.GetLoggedInProvider();
        if (Provider == null) await OnBack();

        if (string.IsNullOrEmpty(AppServices.Current.AppPreference.PushToken))
            AppServices.Current.AppPreference.PushToken = await App.FirebasePushNotificationService.GetToken();
    }

    private async Task Login(bool isSuccess = false)
    {
#if !DEBUG && !TEST
        if (DeviceInfo.Current.DeviceType != DeviceType.Physical)
        {
            await App.Current.MainPage.DisplayAlert(Messages.Error, Messages.NoPhysicalDevice, Messages.Ok);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
#endif

        SystemHelper.Current.HideKeyboard();
        await Task.Delay(200);

        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.NoInternet, false);
            return;
        }

        var loginCount = await SecureStorage.Default.GetAsync(Constants.KeyInvLoginCount);
        var lastLogin = await SecureStorage.Default.GetAsync(Constants.KeyInvLoginTime);

        if (loginCount == null || Convert.ToInt32(loginCount) < Constants.LoginValidAttempts ||
            (DateTimeExtensions.NowNoTimezone() - Convert.ToDateTime(lastLogin)).TotalMinutes > Constants.LoginBlockedInMins)
        {
            var isTampered = await AppServices.Current.TamperingService.IsTimeTampered();
            if (isTampered)
            {
                await Application.Current.MainPage.ShowSnackbar(Messages.DateTimeTampered, false, true);
                await AppServices.Current.AuthService.Logout();
                return;
            }

            var profile = await AppServices.Current.AuthService.Login(Email.ToLower(), Password);
            if (profile != null)
            {
                SecureStorage.Default.Remove(Constants.KeyInvLoginCount);
                SecureStorage.Default.Remove(Constants.KeyInvLoginTime);

                if (profile.NeedDeviceRegistration)
                {
                    AppServices.Current.AppPreference.HasBiometricReg = App.BiometricSnapshotService.SnapshotBiometricState();
                    if (!AppServices.Current.AppPreference.HasBiometricReg)
                        await Application.Current.MainPage.ShowSnackbar(Messages.BiometricKeyRegistrationFailed, false, true);
                }

                var syncTask = profile.Type switch
                {
                    nameof(EUserType.CAREWORKER) or nameof(EUserType.NEXTOFKIN) or nameof(EUserType.SERVICEUSER)
                        => AppServices.Current.BookingService.SyncAllBookings(),
                    nameof(EUserType.SUPERVISOR) => AppServices.Current.SupervisorService.SyncData(),
                    _ => Task.CompletedTask
                };
                await syncTask;

                if (profile.Type != nameof(EUserType.SUPERVISOR))
                {
                    await AppServices.Current.NotificationService.SyncAll();

                    if (profile.Type == nameof(EUserType.CAREWORKER))
                        await AppServices.Current.CareWorkerService.SyncCareWorker(profile.Id);
                    else if (profile.Type == nameof(EUserType.SERVICEUSER) || profile.Type == nameof(EUserType.NEXTOFKIN))
                        await AppServices.Current.ServiceUserService.SyncServiceUser(profile.Id);
                }

                await AppData.Current.InitializeAsync();
                Application.Current.MainPage = profile.Type switch
                {
                    nameof(EUserType.CAREWORKER) => _careWorkerHomePage,
                    nameof(EUserType.SUPERVISOR) => _supervisorHomePage,
                    nameof(EUserType.NEXTOFKIN) or nameof(EUserType.SERVICEUSER) => _serviceUserHomePage,
                    _ => _loginProviderPage,
                };
            }
            else
            {
                await SecureStorage.Default.SetAsync(Constants.KeyInvLoginCount, !string.IsNullOrEmpty(loginCount) &&
                                                (DateTimeExtensions.NowNoTimezone() - Convert.ToDateTime(lastLogin)).TotalMinutes <
                                                Constants.LoginBlockedInMins ? (Convert.ToInt32(loginCount) + 1).ToString() : 1.ToString());
                await SecureStorage.Default.SetAsync(Constants.KeyInvLoginTime, DateTimeExtensions.NowNoTimezone().ToString());

                await Application.Current.MainPage.ShowSnackbar(Messages.LoginFailed, false);
            }
        }
        else await Application.Current.MainPage.ShowSnackbar(Messages.LoginMultipleAttempts, false);
    }

    protected new async Task OnBack()
    {
        await AppServices.Current.ProviderService.DeleteAll();
        SecureStorage.Default.Remove(Constants.KeyAppLocation);

        Application.Current.MainPage = ServiceLocator.GetService<LoginProviderPage>();
    }

    private async Task ForgotPassword()
    {
        var provider = await AppServices.Current.ProviderService.GetLoggedInProvider();
        if (provider == null) return;

        await Browser.OpenAsync($"https://vsauth.{provider.Identifier}{provider.CookieDomain}/{Constants.TpUrlForgotPassword}");
    }
}