using static Microsoft.Maui.ApplicationModel.Permissions;
#if IOS
using UIKit;
#endif

namespace VisitTracker;

/// <summary>
/// SystemHelper is a utility class that provides various helper methods for system-related tasks.
/// It includes methods for verifying biometric authentication, checking and requesting permissions, opening phone dialer, handling cookies for third-party URLs, and tracking errors.
/// </summary>
public class SystemHelper
{
    public static SystemHelper Current => ServiceLocator.GetService<SystemHelper>();

    /// <summary>
    /// Verifies biometric authentication by checking the hardware status and performing authentication if necessary.
    /// </summary>
    /// <returns>VisitBiometricDto object containing the biometric status and response details</returns>
    public async Task<VisitBiometricDto> VerifyBiometric()
    {
        var biometricStatus = await BiometricAuthenticationService.Default.GetAuthenticationStatusAsync();
        AuthenticationResponse biometricResponse = null;
        if (biometricStatus == BiometricHwStatus.Success && AppServices.Current.AppPreference.HasBiometricReg)
        {
            var authenticationRequest = new AuthenticationRequest
            {
                AllowPasswordAuth = false,
                Title = "Authenticate",
                Subtitle = "Please authenticate before starting visit",
                NegativeText = "Use Password",
                Description = "Biometric authentication is required for Start Visit",
                AuthStrength = AuthenticatorStrength.Strong
            };
            biometricResponse = await BiometricAuthenticationService.Default.AuthenticateAsync(authenticationRequest, CancellationToken.None);
        }

        var biometricDto = new VisitBiometricDto
        {
            IsRegistrationSucess = AppServices.Current.AppPreference.HasBiometricReg,
            HasNewEnrolments = App.BiometricSnapshotService.HasBiometricStateChanged(),
            HardwareStatus = biometricStatus.ToString(),
            ResponseStatus = biometricResponse?.Status.ToString(),
            AuthenticationType = biometricResponse?.AuthenticationType.ToString(),
            ErrorMessage = biometricResponse?.ErrorMsg?.ToString(),
        };

        return biometricDto;
    }

    /// <summary>
    /// Checks and requests permissions for various system features such as microphone, camera, location, sensors, and storage.
    /// It uses the Permissions API to check the status of each permission and requests it if not granted.
    /// </summary>
    /// <param name="permission"></param>
    /// <returns>true if permission is granted</returns>
    public async Task<bool> CheckAndGetPermissionsAsync(BasePermission permission)
    {
        PermissionStatus status;
        switch (permission)
        {
            case Microphone:
                status = await CheckStatusAsync<Microphone>();
                if (status != PermissionStatus.Granted)
                {
                    status = await RequestAsync<Microphone>();
                    if (status != PermissionStatus.Granted) return true;
                    else return false;
                }
                else return true;

            case Camera:
                status = await CheckStatusAsync<Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await RequestAsync<Camera>();
                    if (status != PermissionStatus.Granted) return true;
                    else return false;
                }
                else return true;

            case LocationAlways:
                status = await CheckStatusAsync<LocationAlways>();
                if (status != PermissionStatus.Granted)
                {
                    status = await RequestAsync<LocationAlways>();
                    if (status != PermissionStatus.Granted) return true;
                    else return false;
                }
                else return true;

            case LocationWhenInUse:
                status = await CheckStatusAsync<LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await RequestAsync<LocationWhenInUse>();
                    if (status != PermissionStatus.Granted) return true;
                    else return false;
                }
                else return true;

            case Sensors:
                status = await CheckStatusAsync<Sensors>();
                if (status != PermissionStatus.Granted)
                {
                    status = await RequestAsync<Sensors>();
                    if (status != PermissionStatus.Granted) return true;
                    else return false;
                }
                else return true;

            case StorageWrite:
                status = await CheckStatusAsync<StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    status = await RequestAsync<StorageWrite>();
                    if (status != PermissionStatus.Granted) return true;
                    else return false;
                }
                else return true;

            case StorageRead:
                status = await CheckStatusAsync<StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    status = await RequestAsync<StorageRead>();
                    if (status != PermissionStatus.Granted) return true;
                    else return false;
                }
                else return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Opens the phone dialer with the specified phone number.
    /// If the phone number is null or empty, it shows a snackbar message indicating that the phone number was not found.
    /// </summary>
    /// <param name="phoneNo">Phone number to default phone dialer with</param>
    /// <returns></returns>
    public async Task Open(string phoneNo)
    {
        if (string.IsNullOrEmpty(phoneNo))
        {
            await App.Current.MainPage.ShowSnackbar(Messages.PhoneNoNotFound, false);
            return;
        }

        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(phoneNo);
        else await App.Current.MainPage.DisplayAlert(Messages.FeatureNotSupported, phoneNo, Messages.Ok);
    }

    /// <summary>
    /// Opens a third-party URL with the specified cookies for authentication.
    /// This method creates a CookieContainer and adds cookies for user authentication, token, and other relevant information.
    /// </summary>
    /// <param name="url">Third party URL to open</param>
    /// <returns>Returns a Cookie Container with auth information</returns>
    public CookieContainer OpenTpUrl(string url)
    {
        CookieContainer cookieContainer = new CookieContainer();

        Uri uri = new Uri(url, UriKind.Absolute);

        cookieContainer.Add(uri, new Cookie
        {
            Name = "userName",
            Value = AppData.Current.CurrentProfile.UserName,
            Path = "/"
        });

        cookieContainer.Add(uri, new Cookie
        {
            Name = "token",
            Value = AppData.Current.CurrentProfile.ThirdPartyToken,
            Path = "/"
        });

        cookieContainer.Add(uri, new Cookie
        {
            Name = "hideNav",
            Value = "true",
            Path = "/"
        });

        cookieContainer.Add(uri, new Cookie
        {
            Name = "lastLogin",
            Value = AppData.Current.CurrentProfile.LastLogin,
            Path = "/"
        });

        cookieContainer.Add(uri, new Cookie
        {
            Name = "isSuperUser",
            Value = AppData.Current.CurrentProfile.IsSuperUser,
            Path = "/"
        });

        cookieContainer.Add(uri, new Cookie
        {
            Name = "isFormManager",
            Value = AppData.Current.CurrentProfile.IsFormManager,
            Path = "/"
        });

        cookieContainer.Add(uri, new Cookie
        {
            Name = "role",
            Value = AppData.Current.CurrentProfile.Role,
            Path = "/"
        });

        return cookieContainer;
    }

    public string GetUrl(string relativeUrl, bool isTools = false)
    {
        var UserType = AppData.Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER) ? "vscw" : "vssu";
        return $"https://{(isTools ? "vstools" : UserType)}.{AppData.Current.Provider.Identifier}{AppData.Current.Provider.CookieDomain}/{relativeUrl}";
    }

    /// <summary>
    /// Tracks errors by capturing exceptions and adding context information such as profile details, exception message, stack trace, and machine info.
    /// This method uses Sentry SDK to capture the exception and send it to the Sentry server for monitoring and analysis.
    /// </summary>
    /// <param name="ex">Exception to track</param>
    public async Task TrackError(Exception ex)
    {
        var profile = await AppServices.Current.ProfileService.GetLoggedInProfileUser();
        var exceptionData = new Dictionary<string, object>
        {
            { "profile", profile != null ? JsonExtensions.Serialize(profile) : null },
            { "Exception", ex?.Message?.ToString() },
            { "StackTrace", ex?.StackTrace?.ToString() },
            { "MachineInfo",   string.Join("|", [DeviceInfo.Manufacturer, DeviceInfo.DeviceType, DeviceInfo.Idiom, DeviceInfo.Platform, DeviceInfo.Model, DeviceInfo.VersionString]) }
        };
        ex.AddSentryContext("Context", exceptionData);
        SentrySdk.CaptureException(ex);
    }

    /// <summary>
    /// Logs out the user by syncing data, deleting all sync data, and clearing the current profile and provider information.
    /// It also refreshes the dashboard view models and navigates to the login page.
    /// </summary>
    public async Task Logout()
    {
        await AppServices.Current.SyncService.SyncData();
        await AppServices.Current.SyncService.DeleteAllBySyncData();

        var isLogoutSuccess = await AppServices.Current.AuthService.Logout();
        if (isLogoutSuccess)
        {
            App.FirebasePushNotificationService.ClearAll();
            var loggedInUserType = AppData.Current.CurrentProfile.Type;
            AppData.Current.Clear();

            if (loggedInUserType == EUserType.CAREWORKER.ToString())
            {
                var dashboardVm = ServiceLocator.GetService<CareWorkerDashboardVm>();
                if (dashboardVm != null) dashboardVm.RefreshOnAppear = true;
                var ongoingVm = ServiceLocator.GetService<OngoingVm>();
                if (ongoingVm != null) ongoingVm.RefreshOnAppear = true;
            }

            if (loggedInUserType == EUserType.SERVICEUSER.ToString()
                || loggedInUserType == EUserType.NEXTOFKIN.ToString())
            {
                var dashboardVm = ServiceLocator.GetService<ServiceUserDashboardVm>();
                if (dashboardVm != null) dashboardVm.RefreshOnAppear = true;
            }

            if (loggedInUserType == EUserType.SUPERVISOR.ToString())
            {
                var dashboardVm = ServiceLocator.GetService<SupervisorDashboardVm>();
                if (dashboardVm != null) dashboardVm.RefreshOnAppear = true;
                var serviceUsersVm = ServiceLocator.GetService<ServiceUsersVm>();
                if (serviceUsersVm != null) serviceUsersVm.RefreshOnAppear = true;
                var careworkersVm = ServiceLocator.GetService<CareWorkersVm>();
                if (careworkersVm != null) careworkersVm.RefreshOnAppear = true;
            }
            else
            {
                var bookingsVm = ServiceLocator.GetService<BookingsVm>();
                if (bookingsVm != null) bookingsVm.RefreshOnAppear = true;
                var notificationsVm = ServiceLocator.GetService<NotificationsVm>();
                if (notificationsVm != null) notificationsVm.RefreshOnAppear = true;
            }

            var miscellaneousVm = ServiceLocator.GetService<MiscellaneousVm>();
            if (miscellaneousVm != null) miscellaneousVm.RefreshOnAppear = true;

            var loginPage = ServiceLocator.GetService<LoginPage>();
            Application.Current.MainPage = loginPage;
        }
        else await App.Current.MainPage.ShowSnackbar(Messages.LogoutFailed, false);
    }

    /// <summary>
    /// Hides the keyboard on physical devices.
    /// This method is used to dismiss the soft keyboard when it is no longer needed.
    /// </summary>
    public void HideKeyboard()
    {
#if ANDROID
        var imm = (Android.Views.InputMethods.InputMethodManager)MauiApplication.Current.GetSystemService(Android.Content.Context.InputMethodService);

        if (imm != null)
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            Android.OS.IBinder wToken = activity.CurrentFocus?.WindowToken;

            if (wToken != null)
                imm.HideSoftInputFromWindow(wToken, 0);
        }
#endif

#if IOS
        UIKit.UIApplication.SharedApplication.KeyWindow?.RootViewController?.View?.EndEditing(true);
#endif
    }

    /// <summary>
    /// Calculates the height available for the card view by subtracting the reserved layout space .
    /// </summary>
    public double SetCardViewHeight(Double reservedHeight)
    {
        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        var cardHeight = screenHeight - reservedHeight;
        return  cardHeight;
    }

    /// <summary>
    /// Calculates the width available for the card view by subtracting the reserved layout space .
    /// </summary>
    public double SetCardViewWidth(double reservedWidthOrPercent, bool isPercentage = false, double maxCardWidth = double.MaxValue)
    {
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        // Use percentage or fixed dp
        double reservedWidth = isPercentage
            ? screenWidth * (reservedWidthOrPercent / 100.0)
            : reservedWidthOrPercent;

        double cardWidth = screenWidth - reservedWidth;

        return Math.Min(cardWidth, maxCardWidth);
    }

    /// <summary>
    /// Calculates the height available for the card view by subtracting the reserved layout space.
    /// </summary>
    public double SetCardViewHeight(double reservedHeightOrPercent, bool isPercentage = false, double maxCardHeight = double.MaxValue)
    {
        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

        // Use percentage or fixed dp
        double reservedHeight = isPercentage
            ? screenHeight * (reservedHeightOrPercent / 100.0)
            : reservedHeightOrPercent;

        double cardHeight = screenHeight - reservedHeight;

        return Math.Min(cardHeight, maxCardHeight);
    }
}