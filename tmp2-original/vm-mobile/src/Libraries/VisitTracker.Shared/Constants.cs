namespace VisitTracker.Shared;

public static class Constants
{
    public static readonly string AppName = "Visit Tracker";

    //Application's URL Data
    public static readonly string UrlServer = "https://staging.api.vm.artivis.co.uk/api";

    public static readonly string UrlTestServer =  "http://10.0.2.2:8081/api"; // Use 10.0.2.2 for Android emulator

    public static readonly string ProviderLoginServer = "https://deploy.api.vs.artivis.co.uk/api";

    public static readonly Encoding UrlEncoding = Encoding.UTF8;
    public static readonly string UrlContentType = "application/json";
    public static readonly int UrlResponseTimeOut = 120;

    public static readonly string SyncFpAction = "FP";
    public static readonly string SyncNotificationAction = "NT";
    public static readonly string SyncSensorData = "VT";
    public static readonly string SyncSupSensorData = "SVT";

    #region Application's End points

    public static readonly string EndUrlProviderLogin = "providerlogin";

    public static readonly string EndUrlLogin = "Auth/Login";
    public static readonly string EndUrlLogout = "Auth/Logout";
    public static readonly string EndUrlRegisterDevice = "Auth/RegisterDevice";
    public static readonly string EndUrlRefreshToken = "Auth/RefreshToken";
    public static readonly string EndUrlServerTime = "Auth/GetServerTime";
    public static readonly string EndUrlServerAvailability = "Auth/CheckServerAvailability";
    public static readonly string EndUrlTpToken = "Auth/GetTpTokenByUser";

    public static readonly string EndUrlGetSignedAttachments = "Attachment/GetSignedAttachments";
    public static readonly string EndUrlGetSignedAttachmentsByUrl = "Attachment/GetSignedAttachmentsByUrl";
    public static readonly string EndUrlGetPresignedUrlsForUpload = "Attachment/GetPresignedUrlsForUpload";

    public static readonly string EndUrlFluidChart = "Fluid/GetFluidChartByServiceUser";

    public static readonly string EndUrlDownloadRoster = "Booking/DownloadRoster";
    public static readonly string EndUrlBookings = "Booking/GetAll";
    public static readonly string EndUrlBooking = "Booking/Get";
    public static readonly string EndUrlBookingEditAccess = "Booking/CheckBookingEditAccess";
    public static readonly string EndUrlCheckMasterCwChange = "Booking/CheckMasterCwChange";
    public static readonly string EndUrlGetHandOverNotes = "Booking/GetHandOverNotes";
    public static readonly string EndUrlSuBookings = "Booking/BookingsByServiceUser";
    public static readonly string EndUrlBookingsBySuAndDate = "Booking/GetByServiceUserAndDate";
    public static readonly string EndUrlBookingsByCwAndDate = "Booking/GetByCareWorkerAndDate";

    public static readonly string EndUrlIncidentReportAdhoc = "Incident/IncidentReportAdhoc";
    public static readonly string EndUrlIncidentsByServiceUser = "Incident/GetAllByServiceUser";
    public static readonly string EndUrlIncidentsByCareWorker = "Incident/GetAllByCareWorker";
    public static readonly string EndUrlIncidentsGetDetail = "Incident/GetIncidentDetail";

    public static readonly string EndUrlMarChart = "Medication/GetMedicationChartByServiceUser";
    public static readonly string EndUrlMed = "Medication/GetMedicationById";
    public static readonly string EndUrlMedAdminReq = "Medication/ReqMedAdministration";

    public static readonly string EndUrlGetNotification = "Notification/GetAll";
    public static readonly string EndUrlNotificationGetFromLastId = "Notification/GetFromLastId";

    public static readonly string EndUrlGetProfileServiceUser = "Profile/GetServiceUser";
    public static readonly string EndUrlGetProfileCareWorker = "Profile/GetCareWorker";

    public static readonly string EndUrlSupDownloadAll = "Supervisor/DownloadAll";
    public static readonly string EndUrlSupFormsCount = "Supervisor/GetFormDetailsBySupervisor";
    public static readonly string EndUrlSupStartVisit = "Supervisor/StartVisit";
    public static readonly string EndUrlSupSubmitVisitReport = "Supervisor/SubmitVisitReport";
    public static readonly string EndUrlSupStartVisitCheck = "Supervisor/CanSupStartVisit";
    public static readonly string EndUrlUpdateGroundTruth = "Supervisor/UpdateGroundTruth";

    public static readonly string EndUrlVisitMessages = "VisitMessage/GetAll";

    public static readonly string EndUrlDownloadVisits = "Visit/DownloadVisits";
    public static readonly string EndUrlCanStartVisit = "Visit/CanStartVisit";
    public static readonly string EndUrlSyncVisit = "Visit/SyncVisit";
    public static readonly string EndUrlPingLocation = "Visit/PingLocation";
    public static readonly string EndUrlCanSubmitReport = "Visit/CanSubmitReport";
    public static readonly string EndUrlSubmitVisitReport = "Visit/SubmitVisitReport";
    public static readonly string EndUrlSubmitVisitEditReport = "Visit/SubmitVisitEditReport";

    public static readonly string EndUrlSupSyncData = "Supervisor/SupervisorSyncUpload";
    public static readonly string EndUrlSuSyncData = "Visit/ServiceUserSyncUpload";
    public static readonly string EndUrlSyncData = "Visit/CareWorkerSyncUpload";

    #endregion Application's End points

    public static readonly string TpUrlProfile = "profile";
    public static readonly string TpUrlApplyLeave = "leave";
    public static readonly string TpUrlAvailability = "availability";
    public static readonly string TpUrlContacts = "contacts";
    public static readonly string TpUrlBookingPreferences = "preferences";
    public static readonly string TpUrlTraining = "training";
    public static readonly string TpUrlLogRequest = "log-request";
    public static readonly string TpUrlForms = "form/requests";
    public static readonly string TpUrlOBAForms = "form/oba";
    public static readonly string TpUrlNotifications = "notifications";
    public static readonly string TpUrlForgotPassword = "forgot-password";

    //Secure Storage Keys
    public static readonly string KeyAppLocation = "app-city-server";

    public static readonly string KeyInvLoginProvCount = "app-invlogprovcnt";
    public static readonly string KeyInvLoginProvTime = "app-invlogprovtime";
    public static readonly string KeyInvLoginCount = "app-invlogcnt";
    public static readonly string KeyInvLoginTime = "app-invlogtime";
    public static readonly string KeyAppToken = "app-utoken";
    public static readonly string KeyAppTokenExpiry = "app-utoken-exp";
    public static readonly string KeyAppRefreshToken = "app-rtoken";
    public static readonly string KeyAppRefreshTokenExpiry = "app-rtoken-exp";

    public static readonly string BodyMapFront = "Front";
    public static readonly string BodyMapBack = "Rear";

    public static readonly string PrefKeyEditBookingId = "KEY_EDIT_BOOKING_ID";
    public static readonly string PrefKeyEditBookingVisitContent = "KEY_EDIT_BOOKING_VISIT_CONTENT";
    public static readonly string PrefKeyLocationUpdates = "KEY_LOC_UPD";
    public static readonly string PrefKeyLocationFpUpdates = "KEY_LOC_FP_UPD";
    public static readonly string PrefKeyShowBookingIncomplete = "KEY_BOOKING_INC_DEP";
    public static readonly string PrefKeyOngoingBookingDetailId = "KEY_ONG_BOOK_ID";
    public static readonly string PrefKeyOngoingBookingTrackingMode = "KEY_ONG_BOOK_TRA";
    public static readonly string PrefKeyOngoingSupervisorVisitId = "KEY_ONG_SUP_ID";
    public static readonly string PrefKeyOngoingFpServiceUserId = "KEY_ONGFP_SU_ID";
    public static readonly string PrefKeyOngoingNonMasterCwBookingId = "KEY_ONG_NONMAST_CW_BKGID";

    //Login Constants
    public static readonly int ProviderLoginValidAttempts = 5;

    public static readonly int ProviderLoginBlockedInMins = 5;
    public static readonly int LoginValidAttempts = 12;
    public static readonly int LoginBlockedInMins = 10;

    //Sensors Constants
    public static readonly int AzimuthBufferCapacity = 1000;

    public static readonly int AccelerationBufferCapacity = 50;
    public static readonly double StepLengthInMetres = 0.67;
    public static readonly double EarthRadius = 6378000.0;

    public static readonly int DrActivationNSteps = 5;
    public static readonly int DrActivationNSeconds = 8;
    public static readonly int DrActivationMaxNSeconds = 16;

    //Booking Constants
    public static readonly int StartVisitNextBookingThresholdInMins = 90;

    public static readonly int NextBookingScopeInMins = 60;

    //Tracker Constants
    public static readonly int PingEveryNMinUpload = 1; // TODO: Should be 5 mins, For now keep it 1 min

    public static readonly int FpIntervalInMs = 10 * 1000;
    public static readonly int FpTotalReadings = 30; // GPS Reading - (FpTotalReading / 2) && Network Reading - (FpTotalReading / 2)
    public static readonly int FpGpsTimeoutInMs = 5 * 60 * 1000;

    public static readonly int GeoBoundaryExternalInMetres = 300;
    public static readonly int GeoBoundaryStartVisitInMetres = 100;
    public static readonly int GeoBoundaryStartVisitGTolerance = 20;

    public static readonly int TimeoutVisitStartInMins = 60;
    public static readonly int TimeoutVisitUploadInMins = 30;

    //Timers and Intervals
    public static readonly int AutoSensingTimerIntervalInMs = 2 * 60 * 1000;

    public static readonly int NormalModeGpsIntervalInMs = 30 * 1000;
    public static readonly int NormalModeNetworkIntervalInMs = NormalModeGpsIntervalInMs * 2;
    public static readonly int TrackingModeInterval1hrInMs = 10 * 1000;
    public static readonly int TrackingModeInterval2hrInMs = 15 * 1000;
    public static readonly int TrackingModeInterval4hrInMs = 30 * 1000;
    public static readonly int TrackingModeInterval12hrInMs = 45 * 1000;
    public static readonly int MinimumDistanceChangeInMeters = 0;

    //Display Parameters
    public static readonly int NoOfDaysMarChart = 30;

    public static readonly int NoOfDaysBookingsToShow = 14;
    public static readonly int NoOfDaysBookingIncidentToPick = 180;

    public static readonly int SnackbarSuccessTimeInSecs = 3;
    public static readonly int SnackbarFailureTimeInSecs = 6;
    public static readonly int SnackbarLongerByMultiplication = 2;

    //Misc Constants
    public static readonly int TimeTamperingToleranceInSecs = 90;

    public static readonly int GroundTruthValidationCheckInMeters = 500;
    public static readonly string S3BucketAppPrefix = "VM";

    //Past report Constants
    public static readonly int DefaultBookingPastDays = 15;
}