namespace VisitTracker;

public partial class App : Application
{
    public static CareWorkerLocationService CareWorkerLocationService;
    public static SupervisorLocationService SupervisorLocationService;
    public static FeaturePermissionHandlerService FeaturePermissionHandlerService;
    public static FirebasePushNotificationService FirebasePushNotificationService;
    public static BiometricSnapshotService BiometricSnapshotService;
    public static bool LocationUpdatesServiceBound = false;

    public IServiceProvider ServiceProvider;

    public static bool IsTracking { get; set; }
    public static bool IsFingerprinting { get; set; }

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        ServiceProvider = serviceProvider;

        Routing.RegisterRoute(nameof(LoaderPage), typeof(LoaderPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(LoginProviderPage), typeof(LoginProviderPage));
        Routing.RegisterRoute(nameof(CareWorkerHomePage), typeof(CareWorkerHomePage));
        Routing.RegisterRoute(nameof(SupervisorHomePage), typeof(SupervisorHomePage));
        Routing.RegisterRoute(nameof(ServiceUserHomePage), typeof(ServiceUserHomePage));
        Routing.RegisterRoute(nameof(ServiceUsersPage), typeof(ServiceUsersPage));
        Routing.RegisterRoute(nameof(ServiceUserDetailPage), typeof(ServiceUserDetailPage));
        Routing.RegisterRoute(nameof(CareWorkersPage), typeof(CareWorkersPage));
        Routing.RegisterRoute(nameof(CareWorkerDetailPage), typeof(CareWorkerDetailPage));
        Routing.RegisterRoute(nameof(BookingsPage), typeof(BookingsPage));
        Routing.RegisterRoute(nameof(BookingEditPage), typeof(BookingEditPage));
        Routing.RegisterRoute(nameof(BookingDetailPage), typeof(BookingDetailPage));
        Routing.RegisterRoute(nameof(FluidChartPage), typeof(FluidChartPage));
        Routing.RegisterRoute(nameof(MarChartPage), typeof(MarChartPage));
        Routing.RegisterRoute(nameof(OngoingPage), typeof(OngoingPage));
        Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage));
        Routing.RegisterRoute(nameof(FluidDetailPage), typeof(FluidDetailPage));
        Routing.RegisterRoute(nameof(MedicationDetailPage), typeof(MedicationDetailPage));
        Routing.RegisterRoute(nameof(BodyMapNotesPopup), typeof(BodyMapNotesPopup));
        Routing.RegisterRoute(nameof(BodyMapPage), typeof(BodyMapPage));
        Routing.RegisterRoute(nameof(MiscellaneousPage), typeof(MiscellaneousPage));
        Routing.RegisterRoute(nameof(MiscellaneousDetailPage), typeof(MiscellaneousDetailPage));
        Routing.RegisterRoute(nameof(NotificationsPage), typeof(NotificationsPage));
        Routing.RegisterRoute(nameof(IncidentReportPage), typeof(IncidentReportPage));
        Routing.RegisterRoute(nameof(ErrorPage), typeof(ErrorPage));

        SetMainPage();

        LocationUpdatesServiceBound = true;
        CareWorkerLocationService = new CareWorkerLocationService();
        SupervisorLocationService = new SupervisorLocationService();

        FeaturePermissionHandlerService = new FeaturePermissionHandlerService();
        FirebasePushNotificationService = new FirebasePushNotificationService();
        BiometricSnapshotService = new BiometricSnapshotService();

        Application.Current.UserAppTheme = AppTheme.Light;
    }

    /// <summary>
    /// Sets the main page of the application based on the user's profile type.
    /// It initializes the application and determines which page to display based on the user's login status.
    /// </summary>
    private async void SetMainPage()
    {
        MainPage = ServiceProvider.GetRequiredService<LoaderPage>();

        var currentUser = await AppServices.Current.ProfileService.GetLoggedInProfileUser();
        if (currentUser == null)
        {
            var currentProvider = await AppServices.Current.ProviderService.GetLoggedInProvider();
            MainPage = new NavigationPage(currentProvider == null ?
                            ServiceProvider.GetRequiredService<LoginProviderPage>()
                            : ServiceProvider.GetRequiredService<LoginPage>());
        }
        else
        {
            await AppData.Current.InitializeAsync();

            Page homePage;
            switch (currentUser.Type)
            {
                case nameof(EUserType.CAREWORKER):
                    homePage = ServiceProvider.GetRequiredService<CareWorkerHomePage>();
                    break;

                case nameof(EUserType.SUPERVISOR):
                    homePage = ServiceProvider.GetRequiredService<SupervisorHomePage>();
                    break;

                case nameof(EUserType.NEXTOFKIN):
                case nameof(EUserType.SERVICEUSER):
                    homePage = ServiceProvider.GetRequiredService<ServiceUserHomePage>();
                    break;

                default:
                    homePage = new NavigationPage(ServiceProvider.GetRequiredService<LoginProviderPage>());
                    break;
            }

            MainPage = homePage;

            AutoInitializeSensingLogic(currentUser.Type);
        }
    }

    /// <summary>
    /// Initializes the sensing logic for the application after a crash or restart.
    /// This method checks if the user is a care worker or supervisor and initializes the respective tracking service.
    /// </summary>
    /// <param name="userType"></param>
    private async void AutoInitializeSensingLogic(string userType)
    {
        try
        {
            IsTracking = Preferences.Default.Get(Constants.PrefKeyLocationUpdates, false);
            if (IsTracking)
            {
                if (userType == nameof(EUserType.CAREWORKER))
                {
                    int ongoingBookingDetailId = Preferences.Default.Get(Constants.PrefKeyOngoingBookingDetailId, default(int));
                    if (ongoingBookingDetailId != default)
                    {
                        await AppServices.Current.CareWorkerTrackerService.Initialize(ongoingBookingDetailId);
                        AppServices.Current.CareWorkerTrackerService.IsTrackingMode = Preferences.Default.Get(Constants.PrefKeyOngoingBookingTrackingMode, true);

                        var now = DateTimeExtensions.NowNoTimezone();
                        var bookingVisit = AppServices.Current.CareWorkerTrackerService.Visit;
                        if (bookingVisit.StartedOn == null && AppServices.Current.CareWorkerTrackerService.OnGoingBooking.EndTime < now)
                            await AppServices.Current.CareWorkerTrackerService.StopNormalMode(ESensorExitReason.NO_UPLOAD_ACK_TIMEOUT_RESUME);
                        else if (bookingVisit.StartedOn != null && bookingVisit.CompletedOn == null && now > bookingVisit.StartedOn.Value.AddMinutes(Constants.TimeoutVisitStartInMins
                                + (AppServices.Current.CareWorkerTrackerService.OnGoingBooking.EndTime - AppServices.Current.CareWorkerTrackerService.OnGoingBooking.StartTime).TotalMinutes))
                            await AppServices.Current.CareWorkerTrackerService.StopNormalMode(ESensorExitReason.NO_UPLOAD_SV_TIMEOUT_RESUME);
                        else if (AppServices.Current.CareWorkerTrackerService.IsTrackingMode && bookingVisit.CompletedOn != null
                                && now > bookingVisit.CompletedOn.Value.AddMinutes(Constants.TimeoutVisitUploadInMins))
                            await AppServices.Current.CareWorkerTrackerService.StopNormalMode(ESensorExitReason.UPLOAD_NORMAL_TIMEOUT_RESUME);
                        else
                            await AppServices.Current.CareWorkerTrackerService.ResumeNormalMode();
                    }
                }
                else if (userType == nameof(EUserType.SUPERVISOR))
                {
                    int ongoingSupervisorVisitId = Preferences.Default.Get(Constants.PrefKeyOngoingSupervisorVisitId, default(int));
                    if (ongoingSupervisorVisitId != default)
                    {
                        await AppServices.Current.SupervisorTrackerService.Initialize(ongoingSupervisorVisitId);
                        AppServices.Current.SupervisorTrackerService.IsTrackingMode = Preferences.Default.Get(Constants.PrefKeyOngoingBookingTrackingMode, true);

                        if (AppServices.Current.SupervisorTrackerService.SupervisorVisit != null)
                        {
                            var now = DateTimeExtensions.NowNoTimezone();
                            var supervisorVisit = AppServices.Current.SupervisorTrackerService.SupervisorVisit;
                            if (AppServices.Current.SupervisorTrackerService.SupervisorVisit.StartedOn != null &&
                                AppServices.Current.SupervisorTrackerService.SupervisorVisit.CompletedOn == null &&
                                now > AppServices.Current.SupervisorTrackerService.SupervisorVisit.StartedOn.Value.AddMinutes(Constants.TimeoutVisitStartInMins * 2))
                                await AppServices.Current.SupervisorTrackerService.StopNormalMode(ESensorExitReason.NO_UPLOAD_SV_TIMEOUT_RESUME);
                            if (AppServices.Current.SupervisorTrackerService.IsTrackingMode && supervisorVisit.CompletedOn != null
                                    && now > supervisorVisit.CompletedOn.Value.AddMinutes(Constants.TimeoutVisitUploadInMins))
                                await AppServices.Current.SupervisorTrackerService.StopNormalMode(ESensorExitReason.UPLOAD_NORMAL_TIMEOUT_RESUME);
                            else
                                await AppServices.Current.SupervisorTrackerService.ResumeNormalMode();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await SystemHelper.Current.TrackError(ex);
        }
    }

    protected override void OnStart()
    { }

    protected override void OnResume()
    {
        //TODO: following is a temporary fix to make sure data is synced. Need to reduce api call on each resume.
        NotificationHelper.Current.SyncNotificationFromLast();
    }
    /// <summary>
    /// Maintains the Shell tab index order of UI for different user types.
    /// </summary>
    private static Dictionary<string, Dictionary<string, int>> TabIndex =
        new Dictionary<string, Dictionary<string, int>>
        {
            {
                nameof(EUserType.CAREWORKER), new Dictionary<string, int>
                {
                    { nameof(CareWorkerDashboardVm), 0 },
                    { nameof(BookingsVm), 1 },
                    { nameof(OngoingVm), 2 },
                    { nameof(NotificationsVm), 3 },
                    { nameof(MiscellaneousVm), 4 },
                }
            },
            {
                nameof(EUserType.SERVICEUSER), new Dictionary<string, int>
                {
                    { nameof(ServiceUserDashboardVm), 0 },
                    { nameof(BookingsVm), 1 },
                    { nameof(NotificationsVm), 2 },
                    { nameof(MiscellaneousVm), 3 },
                }
            },
            {
                nameof(EUserType.NEXTOFKIN), new Dictionary<string, int>
                {
                    { nameof(ServiceUserDashboardVm), 0 },
                    { nameof(BookingsVm), 1 },
                    { nameof(NotificationsVm), 2 },
                    { nameof(MiscellaneousVm), 3 },
                }
            },
            {
                nameof(EUserType.SUPERVISOR), new Dictionary<string, int>
                {
                    { nameof(SupervisorDashboardVm), 0 },
                    { nameof(ServiceUsersVm), 1 },
                    { nameof(CareWorkersVm), 2 },
                    { nameof(MiscellaneousVm), 3 },
                }
            }
        };

    /// <summary>
    /// Gets the tab index for a specific ViewModel based on the user's profile type.
    /// This method retrieves the tab index from the TabIndex dictionary, which defines the order of tabs for different user types.
    /// </summary>
    /// <param name="ViewModel"></param>
    /// <returns></returns>
    public static async Task<int> GetTabIndexByVm(string ViewModel)
    {
        var currentUser = await AppServices.Current.ProfileService.GetLoggedInProfileUser();
        return TabIndex[currentUser.Type][ViewModel];
    }
}