namespace VisitTracker;

/// <summary>
/// Messages is a static class that contains a collection of string constants used for displaying various messages and notifications in the application.
/// These messages are used throughout the application to provide feedback to the user, such as success or error messages, confirmation dialogs, and other notifications.
/// </summary>
public static class Messages
{
    public static readonly string Yes
        = "Yes";

    public static readonly string No
        = "No";

    public static readonly string Ok
        = "Ok";

    public static readonly string Cancel
        = "Cancel";

    public static readonly string Warning
        = "Warning!";

    public static readonly string Notification
        = "Notification!";

    public static readonly string Message
        = "Message!";

    public static readonly string Error
        = "Error!";

    public static readonly string FeatureNotSupported
        = "Feature not Supported";

    public static readonly string NoAccessPage
        = "Access Denied";

    public static readonly string NoInternet
        = "Internet Not Available";

    public static readonly string ServerUnreachable
        = "Server Unreachable, Please Try Later";

    public static readonly string ExceptionOccurred
        = "System Error, Please Contact Administrator";

    public static readonly string NoPhysicalDevice
        = "Application Not Supported";

    public static readonly string DeleteSuccess
        = "Delete Successful";

    public static readonly string ShortRemarksRequired
        = "Submit Short Remarks";

    public static readonly string HealthStatusRequired
        = "Submit Health Status";

    public static readonly string UnableToEndVisitCwMissing
        = "Upload Failed - Other Care Worker Missing";

    public static readonly string UnableToEndVisitReportMissing
        = "Upload Failed - Visit Report Missing";

    public static readonly string BookingEditAllowed
        = "Edit Enabled";

    public static readonly string BookingEditNotAllowed
        = "Edit Disabled";

    public static readonly string CanOnlyEditLatestVisitReport
        = "Only the latest report version can be edited";

    public static readonly string UploadSuccess
        = "Upload Successful";

    public static readonly string UploadFailure
        = "Upload Failed";

    public static readonly string DialogConfirmationTitle
        = "Confirm?";

    public static readonly string DialogOverrideFingerprint
        = "Overide Existing Fingerprint?";

    public static readonly string DialogCancelFingerprint
        = "Confirm 'Cancel Fingerprint' ?";

    public static readonly string SummaryImageUploadMaxLimit
        = "Max Limit Reached for Image Attachments";

    public static readonly string SummaryAudioUploadMaxLimit
        = "Max Limit Reached for Audio Attachments";

    public static readonly string InvalidAttachment
        = "Invalid Attachment - Contact Administrator";

    public static readonly string NoSuchFileExists
        = "Invalid Attachment - Contact Administrator";

    public static readonly string ProfilePictureNotAvailable
        = "Profile Image Missing";

    public static readonly string LoginForgotProviderCode
        = "Contact Office for Details";

    public static readonly string ProviderCodeInvalid
        = "Invalid Provider Code";

    public static readonly string ProviderMutipleAttempts
        = "Multiple Invalid Attempts - Please Try in " + Constants.ProviderLoginBlockedInMins + " minute(s).";

    public static readonly string LoginFailed
        = "Login Failed - Incorrect Credentials";

    public static readonly string LogoutFailedTracking
        = "Logout Failed - Ongoing Visit Detected";

    public static readonly string LogoutFailed
        = "Logout Failed";

    public static readonly string LogoutFailedPendingUploadConfirm
        = "Pending Data Transfer, Do you still want to logout?";

    public static readonly string LoginProviderFailed
        = "Invalid Provider Code";

    public static readonly string LoginMultipleAttempts
        = "Multiple Invalid Login Attempts - Please Try in " + Constants.LoginBlockedInMins + " Minutes.";

    public static readonly string GroundTruthUpdateSuccess
        = "Ground Truth update successful";

    public static readonly string GroundTruthUpdateFailure
        = "Ground Truth update unsuccessful, Please Try later";

    public static readonly string VisitDuringFingerprint
        = "Start-visit Failed - Fingerprint is Active";

    public static readonly string SupVisitDuringAnotherVisit
        = "Start-visit Unsuccessful - Ongoing Visit Detected";

    public static readonly string NoLocationSelected
        = "Please select the location!";

    public static readonly string FingerprintDuringAnotherFp
        = "Finferprint is Active - Please Try Later";

    public static readonly string FingerprintDuringVisit
        = "Fingerprint Failed - Ongoing Visit Detected";

    public static readonly string FingerprintCompleteSuccess
        = "Fingerprint Completed";

    public static readonly string FingerprintCompleteFailure
        = "Fingerprint Failed";

    public static readonly string BiometricVerificationFailure
        = "Biometric Verification Failed";

    public static readonly string MockLocationDetected
        = "Invalid Location Provider";

    public static readonly string NextBookingThresholdError
        = "Start-visit Failed - Start Time Outside Range";

    public static readonly string BiometricKeyRegistrationFailed
        = "Biometric Registration Failed";

    public static readonly string DateTimeTampered
        = "Incorrect Date/Time Detected";

    public static readonly string BookingRemoved
        = "Booking Removed from Roster";

    public static readonly string BookingNotRelevant
        = "Booking is no longer relevant";

    public static readonly string TaskCompletionBeforeSubmit
        = "Complete All Tasks & Try Again";

    public static readonly string MedicationCompletionBeforeSubmit
        = "Complete All Medication Tasks & Try Again";

    public static readonly string MedicationSubmissionPendingApproval
        = "Approval Pending - Please Try Later";

    public static readonly string SelectStatusBeforeTaskSubmit
        = "Missing Task Completion Status";

    public static readonly string SelectStatusBeforeMedicationSubmit
        = "Missing Medication Completion Status";

    public static readonly string SelectStatusDetailBeforeMedicationSubmit
        = "Missing Medication Completion Detail";

    public static readonly string FluidAnyOneInputRequired
        = "Missing Fluid Intake";

    public static readonly string FluidAnyOneOutputRequired
        = "Missing Fluid Output";

    public static readonly string BodyMapNoSelection
        = "Empty Body Map";

    public static readonly string IncidentInjurySelectionRequired
        = "Missing Injury Type";

    public static readonly string IncidentTypeSelectionRequired
        = "Missing Incident Type";

    public static readonly string IncidentTreatmentSelectionRequired
        = "Missing Treatment Type";

    public static readonly string IncidentOtherTypeRequired
        = "Missing Other Incident Type";

    public static readonly string IncidentLocationRequired
        = "Missing Incident Location";

    public static readonly string IncidentOtherInjuryRequired
        = "Missing Other Injury Type";

    public static readonly string IncidentSummaryRequired
        = "Missing Summary";

    public static readonly string IncidentAdhocUploadFailed
        = "Incident Report Submission Failed";

    public static readonly string DataStoreSuccess
        = "Data Saved";

    public static readonly string PhoneNoNotFound
        = "Phone Number Missing";

    public static readonly string Emergency
        = "Emergency Contacts";

    public static readonly string Ambulance
        = "Ambulance";

    public static readonly string Fire
        = "Fire";

    public static readonly string DialogLogoutConfirmation
        = "Confirm 'Logout'?";

    public static readonly string DialogDeleteConfirmation
        = "Confirm 'Delete'?";

    public static readonly string DialogExitWithoutUploading
        = "Exit Without Saving?";

    public static readonly string DialogSaveConfirmation
        = "Confirm 'Save'?";

    public static readonly string DialogUpdateConfirmation
        = "Confirm 'Update'?";

    public static readonly string LocationPermissionError
        = "This app collects location data for ETA and work hours tracking, even when closed or not in use. Tap here to allow access at all times.";

    public static readonly string LocationEnabledError
        = "Turn on your device's location service.";

    public static readonly string StepCountPermissionError
        = $"Tap here to allow access to {(DeviceInfo.Platform == DevicePlatform.Android ? "Physical activity" : "Motion & fitness")} data.";

    public static readonly string IgnoreBatteryPermissionError
        = "Tap here to disable battery optimization for better performance.";

    public static readonly string NotificationPermissionEnabledError
        = "Tap here to allow notifications.";

    public static readonly string InternetEnabledError
        = "Turn-on Internet";

    public static readonly string Lat
        = "Latitude";

    public static readonly string LatInputMessage
        = "Enter Latitude";

    public static readonly string Lon
        = "Longitude";

    public static readonly string LonInputMessage
        = "Enter Longitude";

    public static readonly string InvalidGroundTruth
        = "Invalid longtitude";

    public static readonly string NoBookingsFound
        = "No bookings on {0}";

    public static readonly string NoIncidentsFound
        = "No Incident Reports";

    public static readonly string GroundTruthFarAwayInDistance
        = "Invalid Ground Truth - Please Contact Office";

    public static readonly string SupVisitStartOutside
        = "Not at Visit Location - Use Away-Actions";

    public static readonly string SupVisitStartOnDifferntDevice
        = "Start Visit Failed - Ongoing visit detected";

    public static readonly Dictionary<string, string> PermissionErrorMessage =
        new()
        {
            { nameof(EFeaturePermissionOrderedType.Location),  LocationPermissionError},
            { nameof(EFeaturePermissionOrderedType.Activity), StepCountPermissionError },
            { nameof(EFeaturePermissionOrderedType.Battery), IgnoreBatteryPermissionError },
            { nameof(EFeaturePermissionOrderedType.Notification), NotificationPermissionEnabledError }
        };

    public static readonly Dictionary<string, string> EnabledErrorMessage =
        new()
        {
            { nameof(EFeaturePermissionOrderedType.Location), LocationEnabledError },
            { nameof(EFeaturePermissionOrderedType.Notification), NotificationPermissionEnabledError },
            { nameof(EFeaturePermissionOrderedType.Internet), InternetEnabledError },
            { nameof(EFeaturePermissionOrderedType.ClockTampered), DateTimeTampered }
        };
}