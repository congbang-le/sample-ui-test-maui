using Microsoft.Maui.Controls.PlatformConfiguration;
using Plugin.Maui.Audio;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace VisitTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSentry(options =>
            {
                options.Dsn = "https://3bc43c62fa989d5533bac6349b41a9e2@o623437.ingest.us.sentry.io/4509076015349760";
            })
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .UseBarcodeReader()
            .AddAudio()
            .RegisterAllServices()
            .ConfigureFonts(fonts =>
            {
                // Fonts
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("SourceSansPro-Regular.ttf", "AppFontFamily");
                fonts.AddFont("SourceSansPro-Bold.ttf", "AppBoldFontFamily");
                fonts.AddFont("Billabong.ttf", "FancyFontFamily");

                // Icon Fonts
                fonts.AddFont("line-awesome.ttf", "LineAwesome");
                fonts.AddFont("fa-regular-400.ttf", "FontawesomeRegular");
                fonts.AddFont("fa-solid-900.ttf", "FontawesomeSolid");
                fonts.AddFont("fa-brands-400.ttf", "FontawesomeBrands");
                fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialCommunityIcons");
                fonts.AddFont("ionicons.ttf", "Ionicons");

                fonts.AddMaterialIconFonts();
            })
            .ConfigureMauiHandlers(h =>
            {
                #if IOS
                    h.AddHandler<Shell, UnifiedShellHandler>();
                #endif

                #if ANDROID
                    h.AddHandler<Shell, TabbarBadgeRenderer>();
                #endif
            }); 

        var app = builder.Build();
        ServiceLocator.Initialize(app.Services);
        return app;
    }

    /// <summary>
    /// Register all services, storages, APIs, view models, and views in the MauiAppBuilder.
    /// This method is called during the application startup process to set up the dependency injection container.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder RegisterAllServices(this MauiAppBuilder builder)
    {
        return builder
            .RegisterAppServices()
            .RegisterStorages()
            .RegisterServices()
            .RegisterApis()
            .RegisterViewModels()
            .RegisterViews();
    }

    /// <summary>
    /// Register application services in the MauiAppBuilder.
    /// This method is responsible for setting up the services that will be used throughout the application.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<IAppPreference, AppPreference>();
        builder.Services.AddTransient<ISecuredStorage, SecuredStorage>();
        builder.Services.AddTransient<ISecuredKeyProvider, SecuredKeyProvider>();
        builder.Services.AddTransient<CommonRequestProvider>();
        builder.Services.AddSingleton<TargetRestServiceRequestProvider>();

        builder.Services.AddSingleton<AppData>();
        builder.Services.AddSingleton<AppServices>();
        builder.Services.AddSingleton<AttachmentHelper>();
        builder.Services.AddSingleton<SystemHelper>();
        builder.Services.AddSingleton<NotificationHelper>();
        builder.Services.AddSingleton<DataRetentionService>();
        builder.Services.AddSingleton<CareWorkerTrackerService>();
        builder.Services.AddSingleton<SupervisorTrackerService>();
        builder.Services.AddSingleton<FingerprintService>();
        builder.Services.AddSingleton<TamperingService>();
        builder.Services.AddSingleton<BackgroundService>();
        builder.Services.AddSingleton<IFileSystemService, FileSystemService>();

        builder.Services.AddSingleton<IBadge>(Badge.Default);

        return builder;
    }

    /// <summary>
    /// Register storages in the MauiAppBuilder.
    /// This method is responsible for setting up the data storage services that will be used throughout the application.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder RegisterStorages(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IAttachmentStorage, AttachmentStorage>();
        builder.Services.AddSingleton<IBodyMapStorage, BodyMapStorage>();
        builder.Services.AddSingleton<IBookingStorage, BookingStorage>();
        builder.Services.AddSingleton<ICareWorkerStorage, CareWorkerStorage>();
        builder.Services.AddSingleton<ICodeStorage, CodeStorage>();
        builder.Services.AddSingleton<IVisitMessageStorage, VisitMessageStorage>();
        builder.Services.AddSingleton<IVisitFluidStorage, VisitFluidStorage>();
        builder.Services.AddSingleton<IIncidentStorage, IncidentStorage>();
        builder.Services.AddSingleton<IMedicationStorage, MedicationStorage>();
        builder.Services.AddSingleton<IVisitMedicationStorage, VisitMedicationStorage>();
        builder.Services.AddSingleton<IMedicationTransactionStorage, MedicationTransactionStorage>();
        builder.Services.AddSingleton<INotificationStorage, NotificationStorage>();
        builder.Services.AddSingleton<IProviderStorage, ProviderStorage>();
        builder.Services.AddSingleton<IServiceUserStorage, ServiceUserStorage>();
        builder.Services.AddSingleton<IServiceUserAddressStorage, ServiceUserAddressStorage>();
        builder.Services.AddSingleton<ITaskStorage, TaskStorage>();
        builder.Services.AddSingleton<IVisitTaskStorage, VisitTaskStorage>();
        builder.Services.AddSingleton<IStepCountStorage, StepCountStorage>();
        builder.Services.AddSingleton<IVisitStorage, VisitStorage>();
        builder.Services.AddSingleton<IBookingDetailStorage, BookingDetailStorage>();
        builder.Services.AddSingleton<ILocationStorage, LocationStorage>();
        builder.Services.AddSingleton<ILocationCentroidStorage, LocationCentroidStorage>();
        builder.Services.AddSingleton<IServiceUserFpStorage, ServiceUserFpStorage>();
        builder.Services.AddSingleton<ISyncStorage, SyncStorage>();
        builder.Services.AddSingleton<IProfileStorage, ProfileStorage>();
        builder.Services.AddSingleton<ISupervisorVisitStorage, SupervisorVisitStorage>();
        builder.Services.AddSingleton<IVisitConsumableStorage, VisitConsumableStorage>();
        builder.Services.AddSingleton<IVisitShortRemarkStorage, VisitShortRemarkStorage>();
        builder.Services.AddSingleton<IVisitHealthStatusStorage, VisitHealthStatusStorage>();

        return builder;
    }

    /// <summary>
    /// Register services in the MauiAppBuilder.
    /// This method is responsible for setting up the services that will be used throughout the application.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IBookingService, BookingService>();
        builder.Services.AddSingleton<ICodeService, CodeService>();
        builder.Services.AddSingleton<IVisitMessageService, VisitMessageService>();
        builder.Services.AddSingleton<IServiceUserService, ServiceUserService>();
        builder.Services.AddSingleton<IServiceUserAddressService, ServiceUserAddressService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<ICareWorkerService, CareWorkerService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IProviderService, ProviderService>();
        builder.Services.AddSingleton<ITaskService, TaskService>();
        builder.Services.AddSingleton<IVisitTaskService, VisitTaskService>();
        builder.Services.AddSingleton<IMedicationService, MedicationService>();
        builder.Services.AddSingleton<IVisitMedicationService, VisitMedicationService>();
        builder.Services.AddSingleton<IMedicationTransactionService, MedicationTransactionService>();
        builder.Services.AddSingleton<IVisitFluidService, VisitFluidService>();
        builder.Services.AddSingleton<IBodyMapService, BodyMapService>();
        builder.Services.AddSingleton<IIncidentService, IncidentService>();
        builder.Services.AddSingleton<IAttachmentService, AttachmentService>();
        builder.Services.AddSingleton<IFluidChartService, FluidChartService>();
        builder.Services.AddSingleton<IMarChartService, MarChartService>();
        builder.Services.AddSingleton<IStepCountService, StepCountService>();
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddSingleton<ILocationCentroidService, LocationCentroidService>();
        builder.Services.AddSingleton<IServiceUserFpService, ServiceUserFpService>();
        builder.Services.AddSingleton<IVisitService, VisitService>();
        builder.Services.AddSingleton<IBookingDetailService, BookingDetailService>();
        builder.Services.AddSingleton<ISyncService, SyncService>();
        builder.Services.AddSingleton<IProfileService, ProfileService>();
        builder.Services.AddSingleton<ISupervisorVisitService, SupervisorVisitService>();
        builder.Services.AddSingleton<ISupervisorService, SupervisorService>();
        builder.Services.AddSingleton<IVisitConsumableService, VisitConsumableService>();
        builder.Services.AddSingleton<IVisitShortRemarkService, VisitShortRemarkService>();
        builder.Services.AddSingleton<IVisitHealthStatusService, VisitHealthStatusService>();

        return builder;
    }

    /// <summary>
    /// Register APIs in the MauiAppBuilder.
    /// This method is responsible for setting up the APIs that will be used throughout the application.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder RegisterApis(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IAttachmentApi, AttachmentApi>();
        builder.Services.AddSingleton<IBookingApi, BookingApi>();
        builder.Services.AddSingleton<IVisitMessageApi, VisitMessageApi>();
        builder.Services.AddSingleton<INotificationApi, NotificationApi>();
        builder.Services.AddSingleton<IAuthApi, AuthApi>();
        builder.Services.AddSingleton<IProviderApi, ProviderApi>();
        builder.Services.AddSingleton<IFluidChartApi, FluidChartApi>();
        builder.Services.AddSingleton<IMarChartApi, MarChartApi>();
        builder.Services.AddSingleton<ILocationCentroidApi, LocationCentroidApi>();
        builder.Services.AddSingleton<ISyncApi, SyncApi>();
        builder.Services.AddSingleton<IVisitApi, VisitApi>();
        builder.Services.AddSingleton<IMedicationApi, MedicationApi>();
        builder.Services.AddSingleton<IIncidentApi, IncidentApi>();
        builder.Services.AddSingleton<ICareWorkerApi, CareWorkerApi>();
        builder.Services.AddSingleton<IServiceUserApi, ServiceUserApi>();
        builder.Services.AddSingleton<ISupervisorApi, SupervisorApi>();
        builder.Services.AddSingleton<ISupervisorVisitApi, SupervisorVisitApi>();

        return builder;
    }

    /// <summary>
    /// Register view models in the MauiAppBuilder.
    /// This method is responsible for setting up the view models that will be used throughout the application.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<LoginProviderVm>();
        builder.Services.AddTransient<LoginProviderQrScannerVm>();
        builder.Services.AddTransient<LoginVm>();
        builder.Services.AddTransient<CareWorkerHomeVm>();
        builder.Services.AddTransient<SupervisorHomeVm>();
        builder.Services.AddTransient<ServiceUserHomeVm>();
        builder.Services.AddSingleton<LoaderVm>();
        builder.Services.AddSingleton<CareWorkerDashboardVm>();
        builder.Services.AddSingleton<SupervisorDashboardVm>();
        builder.Services.AddSingleton<ServiceUserDashboardVm>();
        builder.Services.AddSingleton<ServiceUsersVm>();
        builder.Services.AddTransient<ServiceUserDetailVm>();
        builder.Services.AddSingleton<CareWorkersVm>();
        builder.Services.AddTransient<CareWorkerDetailVm>();
        builder.Services.AddSingleton<BookingsVm>();
        builder.Services.AddTransient<BookingEditVm>();
        builder.Services.AddTransient<BookingDetailVm>();
        builder.Services.AddSingleton<OngoingVm>();
        builder.Services.AddTransient<FluidChartVm>();
        builder.Services.AddTransient<MarChartVm>();
        builder.Services.AddTransient<TaskDetailVm>();
        builder.Services.AddTransient<FluidDetailVm>();
        builder.Services.AddTransient<MedicationDetailVm>();
        builder.Services.AddTransient<BodyMapVm>();
        builder.Services.AddTransient<BodyMapNotesPopupVm>();
        builder.Services.AddSingleton<MiscellaneousVm>();
        builder.Services.AddSingleton<MiscellaneousDetailVm>();
        builder.Services.AddSingleton<NotificationsVm>();
        builder.Services.AddTransient<IncidentReportVm>();
        builder.Services.AddTransient<ErrorVm>();
        builder.Services.AddTransient<HtmlVm>();

        return builder;
    }

    /// <summary>
    /// Register views in the MauiAppBuilder.
    /// This method is responsible for setting up the views that will be used throughout the application.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<LoginProviderPage>();
        builder.Services.AddTransient<LoginProviderQrScannerPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CareWorkerHomePage>();
        builder.Services.AddTransient<SupervisorHomePage>();
        builder.Services.AddTransient<ServiceUserHomePage>();
        builder.Services.AddSingleton<LoaderPage>();
        builder.Services.AddSingleton<CareWorkerDashboardPage>();
        builder.Services.AddSingleton<SupervisorDashboardPage>();
        builder.Services.AddSingleton<ServiceUserDashboardPage>();
        builder.Services.AddSingleton<ServiceUsersPage>();
        builder.Services.AddTransient<ServiceUserDetailPage>();
        builder.Services.AddSingleton<CareWorkersPage>();
        builder.Services.AddTransient<CareWorkerDetailPage>();
        builder.Services.AddSingleton<BookingsPage>();
        builder.Services.AddTransient<BookingEditPage>();
        builder.Services.AddTransient<BookingDetailPage>();
        builder.Services.AddSingleton<OngoingPage>();
        builder.Services.AddTransient<FluidChartPage>();
        builder.Services.AddTransient<MarChartPage>();
        builder.Services.AddTransient<TaskDetailPage>();
        builder.Services.AddTransient<FluidDetailPage>();
        builder.Services.AddTransient<MedicationDetailPage>();
        builder.Services.AddTransient<BodyMapPage>();
        builder.Services.AddTransient<BodyMapNotesPopup>();
        builder.Services.AddSingleton<MiscellaneousPage>();
        builder.Services.AddSingleton<MiscellaneousDetailPage>();
        builder.Services.AddSingleton<NotificationsPage>();
        builder.Services.AddTransient<IncidentReportPage>();
        builder.Services.AddTransient<ErrorPage>();
        builder.Services.AddTransient<HtmlPage>();

        return builder;
    }
}