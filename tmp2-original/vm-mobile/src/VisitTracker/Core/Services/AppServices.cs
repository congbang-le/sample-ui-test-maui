namespace VisitTracker;

/// <summary>
/// AppServices is a singleton class that provides access to various services used in the application.
/// It acts as a central point for managing and accessing different services, such as authentication, booking, and notification services.
/// </summary>
public class AppServices
{
    public static AppServices Current => ServiceLocator.GetService<AppServices>();

    public IBadge AppBadge { get; private set; }

    public IAppPreference AppPreference { get; private set; }
    public CommonRequestProvider CommonRequestProvider { get; private set; }
    public TargetRestServiceRequestProvider TargetRestServiceRequestProvider { get; private set; }
    public CareWorkerTrackerService CareWorkerTrackerService { get; private set; }
    public SupervisorTrackerService SupervisorTrackerService { get; private set; }
    public FingerprintService FingerprintService { get; private set; }
    public TamperingService TamperingService { get; private set; }
    public IFileSystemService FileUploadService { get; private set; }
    public BackgroundService BackgroundService { get; private set; }
    public DataRetentionService DataRetentionService { get; private set; }

    public IAudioManager AudioManager { get; private set; }

    public IAttachmentService AttachmentService { get; private set; }
    public IBookingService BookingService { get; private set; }
    public IBookingDetailService BookingDetailService { get; private set; }
    public IVisitConsumableService VisitConsumableService { get; private set; }
    public IVisitShortRemarkService VisitShortRemarkService { get; private set; }
    public IVisitHealthStatusService VisitHealthStatusService { get; private set; }
    public ICareWorkerService CareWorkerService { get; private set; }
    public ICodeService CodeService { get; private set; }
    public IVisitMessageService VisitMessageService { get; private set; }
    public IBodyMapService BodyMapService { get; private set; }
    public IVisitFluidService VisitFluidService { get; private set; }
    public IFluidChartService FluidChartService { get; private set; }
    public IIncidentService IncidentService { get; private set; }
    public IAuthService AuthService { get; private set; }
    public IMarChartService MarChartService { get; private set; }
    public IMedicationService MedicationService { get; private set; }
    public IVisitMedicationService VisitMedicationService { get; private set; }
    public INotificationService NotificationService { get; private set; }
    public IProviderService ProviderService { get; private set; }
    public IStepCountService StepCountService { get; private set; }
    public IVisitService VisitService { get; private set; }
    public ILocationService LocationService { get; private set; }
    public ILocationCentroidService LocationCentroidService { get; private set; }
    public IServiceUserFpService ServiceUserFpService { get; private set; }
    public IServiceUserService ServiceUserService { get; private set; }
    public IServiceUserAddressService ServiceUserAddressService { get; private set; }
    public ISupervisorService SupervisorService { get; private set; }
    public ISyncService SyncService { get; private set; }
    public ITaskService TaskService { get; private set; }
    public IVisitTaskService VisitTaskService { get; private set; }
    public IProfileService ProfileService { get; private set; }
    public ISupervisorVisitService SupervisorVisitService { get; private set; }

    public AppServices(IBadge badge,
        IAppPreference appPreference,
        CommonRequestProvider commonRequestProvider,
        TargetRestServiceRequestProvider targetRestServiceRequestProvider,
        CareWorkerTrackerService careWorkerTrackerService,
        SupervisorTrackerService supervisorTrackerService,
        FingerprintService fingerprintService,
        TamperingService tamperingService,
        IFileSystemService fileUploadService,
        BackgroundService backgroundService,
        DataRetentionService dataRetentionService,

        IAudioManager audioManager,

        IAttachmentService attachmentService,
        IBookingService bookingService,
        IBookingDetailService bookingDetailService,
        IVisitConsumableService bookingConsumableService,
        IVisitShortRemarkService bookingShortRemarkService,
        IVisitHealthStatusService bookingHealthStatusService,
        ICareWorkerService careWorkerService,
        ICodeService codeService,
        IVisitMessageService visitMessageService,
        IBodyMapService bodyMapService,
        IVisitFluidService fluidService,
        IFluidChartService fluidChartService,
        IIncidentService incidentService,
        IAuthService authService,
        IMarChartService marChartService,
        IMedicationService medicationService,
        IVisitMedicationService visitMedicationService,
        INotificationService notificationService,
        IProviderService providerService,
        IStepCountService stepCountService,
        IVisitService bookingVisitService,
        ILocationService locationService,
        ILocationCentroidService locationCentroidService,
        IServiceUserFpService serviceUserFpService,
        IServiceUserService serviceUserService,
        IServiceUserAddressService serviceUserAddressService,
        ISupervisorService supervisorService,
        ISyncService syncService,
        ITaskService taskService,
        IVisitTaskService visitTaskService,
        IProfileService profileService,
        ISupervisorVisitService supervisorVisitService)
    {
        AppBadge = badge;

        AppPreference = appPreference;
        CommonRequestProvider = commonRequestProvider;
        TargetRestServiceRequestProvider = targetRestServiceRequestProvider;
        CareWorkerTrackerService = careWorkerTrackerService;
        SupervisorTrackerService = supervisorTrackerService;
        FingerprintService = fingerprintService;
        TamperingService = tamperingService;
        FileUploadService = fileUploadService;
        BackgroundService = backgroundService;
        DataRetentionService = dataRetentionService;

        AudioManager = audioManager;

        AttachmentService = attachmentService;
        BodyMapService = bodyMapService;
        BookingService = bookingService;
        BookingDetailService = bookingDetailService;
        VisitConsumableService = bookingConsumableService;
        VisitShortRemarkService = bookingShortRemarkService;
        VisitHealthStatusService = bookingHealthStatusService;
        CareWorkerService = careWorkerService;
        CodeService = codeService;
        VisitMessageService = visitMessageService;
        VisitFluidService = fluidService;
        FluidChartService = fluidChartService;
        IncidentService = incidentService;
        AuthService = authService;
        MarChartService = marChartService;
        MedicationService = medicationService;
        VisitMedicationService = visitMedicationService;
        NotificationService = notificationService;
        ProviderService = providerService;
        StepCountService = stepCountService;
        VisitService = bookingVisitService;
        LocationService = locationService;
        LocationCentroidService = locationCentroidService;
        ServiceUserFpService = serviceUserFpService;
        ServiceUserService = serviceUserService;
        ServiceUserAddressService = serviceUserAddressService;
        SupervisorService = supervisorService;
        SyncService = syncService;
        TaskService = taskService;
        VisitTaskService = visitTaskService;
        ProfileService = profileService;
        SupervisorVisitService = supervisorVisitService;
    }
}