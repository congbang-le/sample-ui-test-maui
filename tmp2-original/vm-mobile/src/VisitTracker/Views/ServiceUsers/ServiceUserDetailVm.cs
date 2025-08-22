namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
public class ServiceUserDetailVm : BaseVm, IQueryAttributable, IDisposable
{
    public int Id { get; set; }

    [Reactive] public ServiceUser ServiceUser { get; set; }
    [Reactive] public ServiceUserAddress ServiceUserAddress { get; set; }
    [Reactive] public UserCardDto ServiceUserCard { get; set; }

    public IList<BookingCardDto> Bookings { get; set; }

    public bool IsBookingNotAvailable { get; set; }
    public bool IsGroundTruthUpdated { get; set; }
    public bool IsIncidentNotAvailable{ get; set; }

    public IList<IncidentResponseDto> Incidents { get; set; }

    public bool IsSupervisor { get; set; }
    public bool IsFpInProgress { get; set; }
    public float FpProgress { get; set; }
    public string GroundLat { get; set; }
    public string GroundLon { get; set; }

    private bool isFirstTimeIncidentsTabSelected = true;
    public string BookingTabText { get; set; } = "Bookings";
    public string IncidentTabText { get; set; } = "Incidents";

    private DateTime? bookingsDate;

    public DateTime? BookingsDate
    {
        get => bookingsDate;
        set
        {
            if (bookingsDate != value)
            {
                bookingsDate = value;
                Bookings = null;

                if (bookingsDate.HasValue && OnBookingsDateChangedCommand.CanExecute.FirstAsync().Wait())
                {
                    OnBookingsDateChangedCommand.Execute().Subscribe();
                }
            }
        }
    }

    public DateTime BookingsMinDate { get; set; }
    public DateTime BookingMaxDate { get; set; }

    public bool AllowStartVisit { get; set; }
    public bool AllowEndVisit { get; set; }

    public ReactiveCommand<Unit, Unit> StartFingerprintCommand { get; }
    public ReactiveCommand<Unit, Unit> StopFingerprintCommand { get; }
    public ReactiveCommand<Unit, Unit> StartVisitCommand { get; }
    public ReactiveCommand<Unit, Unit> EndVisitCommand { get; }
    public ReactiveCommand<Unit, Unit> SubmitFormsCommand { get; }
    public ReactiveCommand<bool, Unit> UpdateGroundTruthCommand { get; }
    public ReactiveCommand<Unit, Unit> OnBookingsDateChangedCommand { get; }
    public ReactiveCommand<Unit, Unit> DatePickedCommand { get; }
    public ReactiveCommand<object, Unit> TabChangeCommand { get; }
    public ReactiveCommand<IncidentResponseDto, Unit> OpenIncidentReportCommand { get; }
    public ReactiveCommand<IncidentResponseDto, Unit> ViewIncidentReportCommand { get; }

    public ServiceUserDetailVm()
    {
        StartFingerprintCommand = ReactiveCommand.CreateFromTask(StartFingerprint);
        StopFingerprintCommand = ReactiveCommand.CreateFromTask(StopFingerprint);
        StartVisitCommand = ReactiveCommand.CreateFromTask(StartVisit);
        EndVisitCommand = ReactiveCommand.CreateFromTask(EndVisit);
        SubmitFormsCommand = ReactiveCommand.CreateFromTask(OpenSubmitFormsByAwayAction);
        UpdateGroundTruthCommand = ReactiveCommand.CreateFromTask<bool>(UpdateGroundTruth);
        OnBookingsDateChangedCommand = ReactiveCommand.CreateFromTask(OnBookingsDateChanged);
        DatePickedCommand = ReactiveCommand.CreateFromTask(OnBookingsDateChanged);
        TabChangeCommand = ReactiveCommand.CreateFromTask<object>(OnTabChange);
        OpenIncidentReportCommand = ReactiveCommand.CreateFromTask<IncidentResponseDto>(OpenIncidentReport);
        ViewIncidentReportCommand = ReactiveCommand.CreateFromTask<IncidentResponseDto>(ViewIncidentReport);

        BindBusyWithException(StartFingerprintCommand);
        BindBusyWithException(StopFingerprintCommand);
        BindBusyWithException(StartVisitCommand);
        BindBusyWithException(EndVisitCommand);
        BindBusyWithException(SubmitFormsCommand);
        BindBusyWithException(UpdateGroundTruthCommand);
        BindBusyWithException(OpenIncidentReportCommand);
        BindBusyWithException(OnBookingsDateChangedCommand);
        BindBusyWithException(TabChangeCommand, true);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();
        SystemHelper.Current.HideKeyboard();

        if ((AppData.Current.CurrentProfile?.Type == nameof(EUserType.SERVICEUSER)
            || AppData.Current.CurrentProfile?.Type == nameof(EUserType.NEXTOFKIN))
            && AppData.Current.CurrentProfile?.Id != Id)
        {
            await Shell.Current.Navigation.PopAsync();
            await Application.Current.MainPage.ShowSnackbar(Messages.NoAccessPage, false);
            return;
        }

        IsSupervisor = AppData.Current.CurrentProfile.Type == EUserType.SUPERVISOR.ToString();

        ServiceUser = await AppServices.Current.ServiceUserService.GetById(Id);
        Title = ServiceUser.Name;
        ServiceUserAddress = await AppServices.Current.ServiceUserAddressService.GetActiveAddressNow(Id);
        ServiceUserCard = await BaseAssembler.BuildUserCardFromSu(ServiceUser, null, ServiceUserAddress);

        var gCentroid = await AppServices.Current.LocationCentroidService.GetGroundTruth(ServiceUser.Id);
        if (gCentroid != null)
        {
            GroundLat = gCentroid.CalibratedLatitude.ToString();
            GroundLon = gCentroid.CalibratedLongitude.ToString();
            IsGroundTruthUpdated = true;
        }
        else IsGroundTruthUpdated = false;

        var currentDateTime = DateTimeExtensions.NowNoTimezone();
        BookingsDate = AppData.Current.CurrentProfile.Type == EUserType.SUPERVISOR.ToString()
            ? currentDateTime : currentDateTime.Date.AddDays(-Constants.DefaultBookingPastDays);

        Incidents = null;
        Bookings = null;

        IsFpInProgress = AppServices.Current.FingerprintService.ServiceUserFp != null &&
            AppServices.Current.FingerprintService.ServiceUserFp.ServiceUserId == ServiceUser.Id;

        if (((ServiceUser.AndroidFpAvailable.HasValue && ServiceUser.AndroidFpAvailable.Value && DeviceInfo.Platform == DevicePlatform.Android) ||
            (ServiceUser.iOSFpAvailable.HasValue && ServiceUser.iOSFpAvailable.Value && DeviceInfo.Platform == DevicePlatform.iOS)) && IsGroundTruthUpdated)
            AllowStartVisit = AppServices.Current.SupervisorTrackerService.SupervisorVisit == null || AppServices.Current.SupervisorTrackerService.SupervisorVisit.ServiceUserId == ServiceUser.Id;
        else AllowStartVisit = false;

        AllowEndVisit = App.IsTracking &&
            AppServices.Current.SupervisorTrackerService.SupervisorVisit != null
            && AppServices.Current.SupervisorTrackerService.SupervisorVisit.ServiceUserId == ServiceUser.Id &&
            AppServices.Current.SupervisorTrackerService.SupervisorVisit.CompletedOn == null;
        InitBookingDate();
    }

    private void InitBookingDate()
    {
        if (AppData.Current.CurrentProfile.Type == EUserType.SUPERVISOR.ToString())
        {
            BookingsMinDate = DateTime.MinValue;
            BookingMaxDate = DateTime.MaxValue;
        }
        else
        {
            BookingsMinDate = DateTimeExtensions.NowNoTimezone().AddDays(-Constants.NoOfDaysBookingsToShow);
            BookingMaxDate = DateTimeExtensions.NowNoTimezone().AddDays(Constants.NoOfDaysBookingsToShow); ;
        }
    }

    private async Task OnTabChange(object parameter)
    {
        if (parameter is UraniumUI.Material.Controls.TabItem tabItem)
        {
            var newTabTitle = tabItem.Title;
            // TODO: Avoid calling the API when changing tabs if the data is already loaded.
            if (newTabTitle == IncidentTabText)
            {
                isFirstTimeIncidentsTabSelected = false;
                Incidents = await AppServices.Current.IncidentService.GetAllByServiceUser(ServiceUser.Id);
                if (Incidents == null || !Incidents.Any())
                {
                    Incidents = null;
                    IsIncidentNotAvailable = true;
                    return;
                }

                IsIncidentNotAvailable = false;
            }
        }
    }

    private async Task OnBookingsDateChanged()
    {
        if (!BookingsDate.HasValue)
            return;

        var bookings = await AppServices.Current.BookingService.GetAllBySuAndDate(ServiceUser.Id, BookingsDate.Value);
        if (bookings == null || !bookings.Any())
        {
            Bookings = null;
            IsBookingNotAvailable = true;
            return;
        }

        Bookings = await BaseAssembler.BuildBookingsCard(bookings);
        IsBookingNotAvailable = false;
    }

    protected async Task UpdateGroundTruth(bool isSuccess = false)
    {
        string lat = await App.Current.MainPage.DisplayPromptAsync(Messages.Lat, Messages.LatInputMessage, initialValue: GroundLat, keyboard: Keyboard.Numeric);
        if (lat == null) return;
        if (lat == string.Empty || lat == "0")
        {
            await App.Current.MainPage.DisplayAlert(Messages.Error, string.Format(Messages.InvalidGroundTruth, "Latitude"), Messages.Ok);
            return;
        }
        GroundLat = lat;

        string lon = await App.Current.MainPage.DisplayPromptAsync(Messages.Lon, Messages.LonInputMessage, initialValue: GroundLon, keyboard: Keyboard.Numeric);
        if (lon == null) return;
        if (lon == string.Empty || lon == "0")
        {
            await App.Current.MainPage.DisplayAlert(Messages.Error, string.Format(Messages.InvalidGroundTruth, "Longitude"), Messages.Ok);
            return;
        }
        GroundLon = lon;

        var groundLoc = new Location(Convert.ToDouble(GroundLat), Convert.ToDouble(GroundLon));
        var serviceUserLoc = new Location(Convert.ToDouble(ServiceUserAddress.Latitude), Convert.ToDouble(ServiceUserAddress.Longitude));
        var distanceInMeters = groundLoc.CalculateDistance(serviceUserLoc, DistanceUnits.Kilometers) * 1000;
        if (distanceInMeters > Constants.GroundTruthValidationCheckInMeters)
        {
            await App.Current.MainPage.DisplayAlert(Messages.Error, Messages.GroundTruthFarAwayInDistance, Messages.Ok);
            return;
        }

        var gCentroid = await AppServices.Current.LocationCentroidService.GetGroundTruth(ServiceUser.Id);
        if (gCentroid == null) gCentroid = new LocationCentroid();

        gCentroid.CalibratedLatitude = gCentroid.FingerprintLatitude = Convert.ToDouble(GroundLat);
        gCentroid.CalibratedLongitude = gCentroid.FingerprintLongitude = Convert.ToDouble(GroundLon);
        gCentroid.LocationClass = ELocationClass.G.ToString();
        gCentroid.ServiceUserId = ServiceUser.Id;
        gCentroid.DeviceInfo = AppServices.Current.AppPreference.DeviceInfo;

        var newCentroid = await AppServices.Current.LocationCentroidService.UpdateGroundTruth(gCentroid);
        await AppServices.Current.LocationCentroidService.InsertOrReplace(newCentroid);
        IsGroundTruthUpdated = true;
        await App.Current.MainPage.ShowSnackbar(newCentroid != null ? Messages.GroundTruthUpdateSuccess : Messages.GroundTruthUpdateFailure,
            newCentroid != null ? true : false);

        await RefreshLinkedPages();
    }

    protected async Task OpenIncidentReport(IncidentResponseDto incident)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            {"ServiceUserId", ServiceUser.Id }
        };
        var IncidentReportDetailpage = $"{nameof(IncidentReportPage)}?";
        await Shell.Current.GoToAsync(IncidentReportDetailpage, navigationParameter);
    }

    protected async Task ViewIncidentReport(IncidentResponseDto incident)
    {
        if (incident == null) return;  

        var navigationParameter = new Dictionary<string, object>
        {
            {"OnlineIncidentId", incident.Id },
            { "IsReadOnly", true }
        };

        var IncidentReportDetailPage = $"{nameof(IncidentReportPage)}?";
        await Shell.Current.GoToAsync(IncidentReportDetailPage, navigationParameter);
    }

    #region Fingerprint

    protected async Task StartFingerprint()
    {
        if (App.IsFingerprinting)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.FingerprintDuringAnotherFp, false);
            return;
        }
        else
        {
            if (App.IsTracking)
            {
                await App.Current.MainPage.ShowSnackbar(Messages.FingerprintDuringVisit, false);
                return;
            }

            var errors = await App.FeaturePermissionHandlerService.CheckEverything();
            if (errors != null && errors.Any())
            {
                await OpenActions.OpenErrorPage();
                return;
            }

            var currentDevicelocation = await GeolocationExtensions.GetBestLocationAsync();
            if (currentDevicelocation.IsFromMockProvider)
            {
                await App.Current.MainPage.ShowSnackbar(Messages.MockLocationDetected, false);
                return;
            }

            bool overrideAccept = true;
            if ((ServiceUser.AndroidFpAvailable.HasValue && ServiceUser.AndroidFpAvailable.Value
                    && DeviceInfo.Platform == DevicePlatform.Android) ||
                (ServiceUser.iOSFpAvailable.HasValue && ServiceUser.iOSFpAvailable.Value
                    && DeviceInfo.Platform == DevicePlatform.iOS))
            {
                overrideAccept = false;
                overrideAccept = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle,
                    Messages.DialogOverrideFingerprint, Messages.Yes, Messages.No);
            }

            if (overrideAccept)
            {
                await DeleteAllPendingFp();
                await AppServices.Current.FingerprintService.StartFingerprint(ServiceUser.Id);
            }
        }
    }

    protected async Task StopFingerprint()
    {
        bool accept = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle,
            Messages.DialogCancelFingerprint, Messages.Yes, Messages.No);
        if (accept) await AppServices.Current.FingerprintService.StopFingerprint(true);
    }

    private async Task DeleteAllPendingFp()
    {
        await AppServices.Current.LocationService.DeleteAllByVisitEmpty();
        await AppServices.Current.ServiceUserFpService.DeleteAll();
    }

    private async Task OnFingerprintStart(bool isSuccess)
    {
        if (AppServices.Current.FingerprintService.ServiceUserFp != null &&
            AppServices.Current.FingerprintService.ServiceUserFp.ServiceUserId == ServiceUser.Id)
        {
            IsFpInProgress = isSuccess;

            if (!isSuccess)
            {
                await DeleteAllPendingFp();
                return;
            }
        }
    }

    private async Task OnFingerprintComplete(bool isSuccess)
    {
        IsFpInProgress = false;

        if (!isSuccess)
        {
            await DeleteAllPendingFp();
            await App.Current.MainPage.ShowSnackbar(Messages.FingerprintCompleteFailure, false, true);
            return;
        }

        await App.Current.MainPage.ShowSnackbar(Messages.FingerprintCompleteSuccess, true);
        await RefreshLinkedPages();
    }

    private async Task OnFingerprintProgress(float progress)
    {
        if (AppServices.Current.FingerprintService.ServiceUserFp != null &&
            AppServices.Current.FingerprintService.ServiceUserFp.ServiceUserId == ServiceUser.Id)
        {
            await Task.Run(() => FpProgress = progress);
        }
    }

    #endregion Fingerprint

    #region Tracking Logic

    private async Task StartVisit()
    {
        var errors = await App.FeaturePermissionHandlerService.CheckEverything();
        if (errors != null && errors.Any())
        {
            var isTampered = errors.Any(x => x.Type == EFeaturePermissionOrderedType.ClockTampered);
            if (isTampered && AppServices.Current.SupervisorTrackerService != null && AppServices.Current.SupervisorTrackerService.SupervisorVisit != null)
            {
                AppServices.Current.SupervisorTrackerService.SupervisorVisit.IsVisitTampered = true;
                AppServices.Current.SupervisorTrackerService.SupervisorVisit = await AppServices.Current.SupervisorVisitService.InsertOrReplace(AppServices.Current.SupervisorTrackerService.SupervisorVisit);
            }

            await OpenActions.OpenErrorPage();
            return;
        }

        if (App.IsFingerprinting)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.VisitDuringFingerprint, false);
            return;
        }
        else if (AppServices.Current.SupervisorTrackerService.SupervisorVisit != null && AppServices.Current.SupervisorTrackerService.SupervisorVisit.Id > 0
                    && AppServices.Current.SupervisorTrackerService.SupervisorVisit.ServiceUserId != ServiceUser.Id)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.SupVisitDuringAnotherVisit, false);
            return;
        }
        else
        {
            var currentDevicelocation = await GeolocationExtensions.GetBestLocationAsync();
            if (currentDevicelocation.IsFromMockProvider)
            {
                await App.Current.MainPage.ShowSnackbar(Messages.MockLocationDetected, false);
                return;
            }

            var canStartVisitResponseDto = await AppServices.Current.SupervisorVisitService.CanSupStartVisit(new SupervisorCheckStartVisitDto
            {
                SupervisorId = AppData.Current.CurrentProfile.Id,
                ServiceUserId = ServiceUser.Id,
                DeviceInfo = DeviceInfo.Platform.ToString(),
                Latitude = currentDevicelocation.Latitude.ToString(),
                Longitude = currentDevicelocation.Longitude.ToString()
            });
            if (canStartVisitResponseDto.IsInsideGeozone && !canStartVisitResponseDto.AnyPendingVisits)
            {
                if (AppServices.Current.SupervisorTrackerService.SupervisorVisit == null)
                {
                    var SupervisorVisit = new SupervisorVisit
                    {
                        ServiceUserId = ServiceUser.Id,
                        SupervisorId = AppData.Current.CurrentProfile.Id,
                        StartedOn = DateTimeExtensions.NowNoTimezone(),
                        IsCompleted = false,
                        MachineInfo = AppServices.Current.AppPreference.DeviceInfo,
                        DeviceInfo = DeviceInfo.Platform.ToString(),
                        StartedLocation = currentDevicelocation.Latitude.ToString() + "," + currentDevicelocation.Longitude.ToString()
                    };
                    SupervisorVisit = await AppServices.Current.SupervisorVisitService.StartVisit(SupervisorVisit);

                    if (AppServices.Current.SupervisorTrackerService.SupervisorVisit != null &&
                            AppServices.Current.SupervisorTrackerService.SupervisorVisit.Id != SupervisorVisit.Id)
                        await AppServices.Current.SupervisorTrackerService.StopNormalMode(ESensorExitReason.NEXT_VISIT_START);

                    await AppServices.Current.SupervisorTrackerService.StartNormalModeBySV(SupervisorVisit.Id);
                    AllowStartVisit = false;
                }
            }
            else if (canStartVisitResponseDto.IsInsideGeozone && canStartVisitResponseDto.AnyPendingVisits)
                await App.Current.MainPage.ShowSnackbar(Messages.SupVisitStartOnDifferntDevice, false);
            else await App.Current.MainPage.ShowSnackbar(Messages.SupVisitStartOutside, false);
        }
    }

    private async Task EndVisit()
    {
        var errors = await App.FeaturePermissionHandlerService.CheckEverything();
        if (errors != null && errors.Any())
        {
            var isTampered = errors.Any(x => x.Type == EFeaturePermissionOrderedType.ClockTampered);
            if (isTampered && AppServices.Current.SupervisorTrackerService != null && AppServices.Current.SupervisorTrackerService.SupervisorVisit != null)
            {
                AppServices.Current.SupervisorTrackerService.SupervisorVisit.IsVisitTampered = true;
                AppServices.Current.SupervisorTrackerService.SupervisorVisit = await AppServices.Current.SupervisorVisitService.InsertOrReplace(AppServices.Current.SupervisorTrackerService.SupervisorVisit);
            }

            await OpenActions.OpenErrorPage();
            return;
        }

        var currentDevicelocation = await GeolocationExtensions.GetBestLocationAsync();
        if (currentDevicelocation.IsFromMockProvider)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.MockLocationDetected, false);
            return;
        }
        var locationList = await AppServices.Current.LocationService.GetBySupVisit(AppServices.Current.SupervisorTrackerService.SupervisorVisit.Id);
        var stepsList = await AppServices.Current.StepCountService.GetBySupVisit(AppServices.Current.SupervisorTrackerService.SupervisorVisit.Id);

        var supVisit = await AppServices.Current.SupervisorVisitService.GetById(AppServices.Current.SupervisorTrackerService.SupervisorVisit.Id);
        supVisit.CompletedOn = DateTimeExtensions.NowNoTimezone();
        if (currentDevicelocation != null) supVisit.UploadedLocation = currentDevicelocation.Latitude + "," + currentDevicelocation.Longitude;

        //Check whether the ongoing notification service can be stopped if app not running on background
        if (AppServices.Current.SupervisorTrackerService.ExitedGeozone.HasValue && AppServices.Current.SupervisorTrackerService.ExitedGeozone.Value)
        {
            supVisit.TerminationReason = ESensorExitReason.UPLOAD_OUTSIDE_EXIT.ToString();
            supVisit.TerminatedOn = DateTimeExtensions.NowNoTimezone();
        }

        supVisit = await AppServices.Current.SupervisorVisitService.InsertOrReplace(supVisit);
        AppServices.Current.SupervisorTrackerService.SupervisorVisit = supVisit;

        var isSubmitSuccess = await AppServices.Current.SupervisorVisitService.SubmitVisitReport(new SupervisorVisitReportDto
        {
            SupervisorVisit = supVisit,
            DoPostProcessing = supVisit.TerminationReason == ESensorExitReason.NO_UPLOAD_SV_TIMEOUT.ToString()
                                ? true : (AppServices.Current.SupervisorTrackerService?.ExitedGeozone ?? false),
            LocationList = locationList?.ToList(),
            StepCountList = stepsList?.ToList()
        });

        await App.Current.MainPage.ShowSnackbar(
            isSubmitSuccess ? Messages.UploadSuccess : Messages.UploadFailure,
            isSubmitSuccess, !isSubmitSuccess);

        if (isSubmitSuccess)
        {
            AllowStartVisit = true;
            AllowEndVisit = false;
            WeakReferenceMessenger.Default.Send(new MessagingEvents.VisitCompletedMessage(true));
        }
    }

    private async Task OpenSubmitFormsByAwayAction()
    {
        await OpenSubmitForms();
    }

    private async Task OpenSubmitForms()
    {
        var url = AppData.Current.ExternalLinks.FirstOrDefault(x => x.LinkType == EExternalLinkType.SUBMIT_FORMS)?.ServerUrl;
        var parameters = new Dictionary<string, string>
        {
            { "about_user", ServiceUser.UserName }
        };

        await OpenActions.OpenMiscDetailPage(url, "Submit Forms", parameters);
    }

    private void OnVisitStart(bool isSuccess)
    {
        if (AppServices.Current.SupervisorTrackerService.SupervisorVisit != null &&
            AppServices.Current.SupervisorTrackerService.SupervisorVisit.ServiceUserId == ServiceUser.Id)
            AllowEndVisit = true;
    }

    private async Task OnVisitComplete(bool isSuccess) => await RefreshLinkedPages();

    #endregion Tracking Logic

    private async Task RefreshLinkedPages()
    {
        var supDashboardVm = ServiceLocator.GetService<SupervisorDashboardVm>();
        if (supDashboardVm != null) supDashboardVm.RefreshOnAppear = true;

        var serviceUsersVm = ServiceLocator.GetService<ServiceUsersVm>();
        if (serviceUsersVm != null) serviceUsersVm.RefreshOnAppear = true;

        await InitCommand.Execute(true);
    }

    public void SubscribeAllMessagingCenters()
    {
        var isStartedFpRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.FingerprintStartedMessage>(this);
        if (!isStartedFpRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.FingerprintStartedMessage>(this,
                async (recipient, message) => await OnFingerprintStart(message.Value));

        var isCompletedFpRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.FingerprintCompletedMessage>(this);
        if (!isCompletedFpRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.FingerprintCompletedMessage>(this,
            async (recipient, message) => await OnFingerprintComplete(message.Value));

        var isProgressFpRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.FingerprintProgressMessage>(this);
        if (!isProgressFpRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.FingerprintProgressMessage>(this,
            async (recipient, message) => await OnFingerprintProgress(message.Value));

        var isStartedVisitRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.SupervisorVisitStartedMessage>(this);
        if (!isStartedVisitRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.SupervisorVisitStartedMessage>(this,
                (recipient, message) => OnVisitStart(message.Value));

        var isCompletedVisitRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.VisitCompletedMessage>(this);
        if (!isCompletedVisitRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.VisitCompletedMessage>(this,
                async (recipient, message) => await OnVisitComplete(message.Value));

        var suBookingEditCompletedRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.ServiceUserDetailPageBookingEditCompleted>(this);
        if (!suBookingEditCompletedRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.ServiceUserDetailPageBookingEditCompleted>(this,
                async (recipient, message) => await InitCommand.Execute(true));
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.FingerprintStartedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.FingerprintCompletedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.FingerprintProgressMessage>(this);

        WeakReferenceMessenger.Default.Unregister<MessagingEvents.SupervisorVisitStartedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.VisitCompletedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.ServiceUserDetailPageBookingEditCompleted>(this);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var param))
            Id = Convert.ToInt32(param.ToString());

        await InitCommand.Execute();
    }
}