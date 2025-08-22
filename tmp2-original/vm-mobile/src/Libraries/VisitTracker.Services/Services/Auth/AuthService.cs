namespace VisitTracker.Services;

public class AuthService : IAuthService
{
    private readonly IAppPreference _appPreference;
    private readonly IAuthApi _api;

    private readonly IFileSystemService _fileSystemService;
    private readonly IAttachmentService _attachmentService;
    private readonly IBookingService _bookingService;
    private readonly IBodyMapService _bodyMapService;
    private readonly IVisitConsumableService _bookingConsumableService;
    private readonly IBookingDetailService _bookingDetailService;
    private readonly IVisitHealthStatusService _bookingHealthStatusService;
    private readonly IVisitShortRemarkService _bookingShortRemarkService;
    private readonly IVisitService _bookingVisitService;
    private readonly ICareWorkerService _careWorkerService;
    private readonly ICodeService _codeService;
    private readonly IVisitFluidService _fluidService;
    private readonly IIncidentService _incidentService;
    private readonly IMedicationService _medicationService;
    private readonly IMedicationTransactionService _medicationTransactionService;
    private readonly INotificationService _notificationService;
    private readonly IProfileService _profileService;
    private readonly IServiceUserService _serviceUserService;
    private readonly ISyncService _syncService;
    private readonly ITaskService _taskService;
    private readonly IVisitMessageService _visitMessageService;
    private readonly IStepCountService _bookingStepCountService;
    private readonly ILocationService _locationService;
    private readonly ILocationCentroidService _locationCentroidService;
    private readonly IServiceUserFpService _serviceUserFpService;
    private readonly ISupervisorVisitService _supervisorVisitService;

    public AuthService(IAppPreference appPreference,
        IAuthApi api,
        IFileSystemService fileSystemService,
        IAttachmentService attachmentService,
        IBookingService bookingService,
        IBodyMapService bodyMapService,
        IVisitConsumableService bookingConsumableService,
        IBookingDetailService bookingDetailService,
        IVisitHealthStatusService bookingHealthStatusService,
        IVisitShortRemarkService bookingShortRemarkService,
        IVisitService bookingVisitService,
        ICareWorkerService careWorkerService,
        ICodeService codeService,
        IVisitFluidService fluidService,
        IIncidentService incidentService,
        IMedicationService medicationService,
        IMedicationTransactionService medicationTransactionService,
        INotificationService notificationService,
        IProfileService profileService,
        IServiceUserService serviceUserService,
        ISyncService syncService,
        ITaskService taskService,
        IVisitMessageService visitMessageService,
        IStepCountService bookingStepCountService,
        ILocationService locationService,
        ILocationCentroidService locationCentroidService,
        IServiceUserFpService serviceUserFpService,
        ISupervisorVisitService supervisorVisitService)
    {
        _appPreference = appPreference;
        _api = api;

        _fileSystemService = fileSystemService;
        _attachmentService = attachmentService;
        _bookingService = bookingService;
        _bodyMapService = bodyMapService;
        _bookingConsumableService = bookingConsumableService;
        _bookingDetailService = bookingDetailService;
        _bookingHealthStatusService = bookingHealthStatusService;
        _bookingShortRemarkService = bookingShortRemarkService;
        _bookingVisitService = bookingVisitService;
        _careWorkerService = careWorkerService;
        _codeService = codeService;
        _fluidService = fluidService;
        _incidentService = incidentService;
        _medicationService = medicationService;
        _medicationTransactionService = medicationTransactionService;
        _notificationService = notificationService;
        _profileService = profileService;
        _serviceUserService = serviceUserService;
        _syncService = syncService;
        _taskService = taskService;
        _visitMessageService = visitMessageService;
        _bookingStepCountService = bookingStepCountService;
        _locationService = locationService;
        _locationCentroidService = locationCentroidService;
        _serviceUserFpService = serviceUserFpService;
        _supervisorVisitService = supervisorVisitService;
    }

    public async Task<Profile> Login(string email, string password)
    {
        _appPreference.DeviceInfo = string.Join("|", [DeviceInfo.Manufacturer, DeviceInfo.DeviceType, DeviceInfo.Idiom, DeviceInfo.Platform, DeviceInfo.Model, DeviceInfo.VersionString]);

        var profile = await _api.GetTokenAndCWDetail(email, password);
        if (profile == null)
            return null;

        profile.UniqueId = _appPreference.DeviceID;
        _appPreference.UserId = profile.Id;
        _appPreference.User = profile.Type;

        await _profileService.DeleteAll();
        if (!string.IsNullOrEmpty(profile.ImageUrl))
        {
            profile.ImageUrl = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(profile.ImageUrl));

            var fileExists = File.Exists(profile.ImageUrl);
            if (!fileExists)
            {
                var attachment = new Attachment
                {
                    S3SignedUrl = profile.SignedImageUrl,
                    S3Url = profile.ImageUrl
                };
                await _fileSystemService.DownloadFile(attachment);
            }
            profile.SignedImageUrl = null;
        }
        return await _profileService.InsertOrReplace(profile);
    }

    public async Task<bool> Logout()
    {
        var serverLogout = await _api.Logout();
        if (serverLogout)
        {
            await _attachmentService.DeleteAll();
            await _bookingService.DeleteAll();
            await _bodyMapService.DeleteAll();
            await _bookingConsumableService.DeleteAll();
            await _bookingDetailService.DeleteAll();
            await _bookingHealthStatusService.DeleteAll();
            await _bookingShortRemarkService.DeleteAll();
            await _bookingVisitService.DeleteAll();
            await _careWorkerService.DeleteAll();
            await _codeService.DeleteAll();
            await _fluidService.DeleteAll();
            await _incidentService.DeleteAll();
            await _medicationService.DeleteAll();
            await _medicationTransactionService.DeleteAll();
            await _notificationService.DeleteAll();
            await _profileService.DeleteAll();
            await _serviceUserService.DeleteAll();
            await _syncService.DeleteAll();
            await _taskService.DeleteAll();
            await _visitMessageService.DeleteAll();
            await _bookingStepCountService.DeleteAll();
            await _locationService.DeleteAll();
            await _locationCentroidService.DeleteAll();
            await _serviceUserFpService.DeleteAll();
            await _supervisorVisitService.DeleteAll();

            _appPreference.Clear();

            Preferences.Default.Remove(Constants.PrefKeyLocationUpdates);
            Preferences.Default.Remove(Constants.PrefKeyLocationFpUpdates);
            Preferences.Default.Remove(Constants.PrefKeyOngoingBookingDetailId);
            Preferences.Default.Remove(Constants.PrefKeyOngoingSupervisorVisitId);
            Preferences.Default.Remove(Constants.PrefKeyOngoingBookingTrackingMode);
            Preferences.Default.Remove(Constants.PrefKeyOngoingFpServiceUserId);
            Preferences.Default.Remove(Constants.PrefKeyShowBookingIncomplete);

            SecureStorage.Default.Remove(Constants.KeyAppToken);
            SecureStorage.Default.Remove(Constants.KeyAppTokenExpiry);
            SecureStorage.Default.Remove(Constants.KeyAppRefreshToken);
            SecureStorage.Default.Remove(Constants.KeyAppRefreshTokenExpiry);
        }

        return serverLogout;
    }

    public async Task<DateTime?> GetServerTime()
    {
        return await _api.GetServerTime();
    }

    public async Task<bool> RegisterDeviceForPushNotifications()
    {
        return await _api.RegisterDeviceForPushNotifications();
    }

    public async Task<bool> CheckServerAvailability()
    {
        return await _api.CheckServerAvailability();
    }

    public async Task<TpAuthResponse> GetTpTokenByUser(int userId)
    {
        return await _api.GetTpTokenByUser();
    }
}