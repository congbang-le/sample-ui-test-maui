namespace VisitTracker;

public class OngoingVm : BaseVm, IDisposable
{
    public static int MaxChars = 2000;
    public bool ShowStartVisitButton { get; set; }
    public OngoingDto OngoingDto { get; set; }
    public Booking Booking { get; set; }
    public string TypeName => EPageType.BookingType.ToString();

    public ReactiveCommand<bool, Unit> SubmitCommand { get; }
    public ReactiveCommand<int, Unit> OpenTaskCommand { get; }
    public ReactiveCommand<int, Unit> OpenFluidDetailCommand { get; }
    public ReactiveCommand<int, Unit> OpenMedicationCommand { get; }
    public ReactiveCommand<Unit, Unit> StartVisitCommand { get; }
    public ReactiveCommand<Unit, Unit> DownloadHandOverNotesCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckMasterCwChangeCommand { get; }
    public ReactiveCommand<Unit, Unit> OnTapContentCommand { get; }

    public OngoingVm()
    {
        SubmitCommand = ReactiveCommand.CreateFromTask<bool>(OnSubmit);
        OpenTaskCommand = ReactiveCommand.CreateFromTask<int>(OpenTask);
        OpenFluidDetailCommand = ReactiveCommand.CreateFromTask<int>(OpenFluidDetail);
        OpenMedicationCommand = ReactiveCommand.CreateFromTask<int>(OpenMedication);
        StartVisitCommand = ReactiveCommand.CreateFromTask(StartVisit);
        DownloadHandOverNotesCommand = ReactiveCommand.CreateFromTask(DownloadHandOverNotes);
        CheckMasterCwChangeCommand = ReactiveCommand.CreateFromTask(CheckMasterCwChange);
        OnTapContentCommand = ReactiveCommand.CreateFromTask(OngoingVm.OnTapContent);

        BindBusyWithException(SubmitCommand);
        BindBusyWithException(OpenTaskCommand);
        BindBusyWithException(OpenFluidDetailCommand);
        BindBusyWithException(OpenMedicationCommand);
        BindBusyWithException(StartVisitCommand);
        BindBusyWithException(DownloadHandOverNotesCommand);
        BindBusyWithException(CheckMasterCwChangeCommand);

        Title = OngoingDto != null && ShowStartVisitButton ? "Ongoing Booking" : "Upcoming Booking";
        var TabTitle = OngoingDto != null && ShowStartVisitButton ? "Ongoing" : "Upcoming";
        WeakReferenceMessenger.Default.Send(new MessagingEvents.PageTabTitleChangeMessage(TabTitle));

    }

    public static Task OnTapContent()
    {
        SystemHelper.Current.HideKeyboard();
        return Task.CompletedTask;
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        if (Preferences.Default.ContainsKey(Constants.PrefKeyOngoingNonMasterCwBookingId))
        {
            var bookingId = Preferences.Default.Get<int>(Constants.PrefKeyOngoingNonMasterCwBookingId, default);
            if (bookingId != default)
                Booking = await AppServices.Current.BookingService.GetById(bookingId);
        }
        else Booking = await AppServices.Current.BookingService.GetCurrentBooking()
                ?? await AppServices.Current.BookingService.GetNextBooking();

        if (Booking != null)
        {
            var bookingDetail = await AppServices.Current.BookingDetailService.GetBookingDetailForCurrentCw(Booking.Id);
            var visit = await AppServices.Current.VisitService.GetByBookingDetailId(bookingDetail.Id);

            if (Booking.IsBookingInScope)
            {
                var IsTracking = Preferences.Default.Get(Constants.PrefKeyLocationUpdates, false);
                if (visit == null && !IsTracking)
                    visit = await AppServices.Current.CareWorkerTrackerService.StartNormalModeByAck(bookingDetail.Id);
            }

            OngoingDto = await BaseAssembler.BuildOngoingViewModels(Booking, visit);

            if (AppData.Current.VisitMessages != null && AppData.Current.VisitMessages.Any())
            {
                OngoingDto.ShortRemarks = BaseAssembler.BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(i => i.Type == EMessageType.SHORT_REMARKS.ToString()).ToList());
                OngoingDto.HealthStatuses = BaseAssembler.BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(i => i.Type == EMessageType.HEALTH_STATUS.ToString()).ToList());
            }

            if (OngoingDto.Visit != null)
            {
                var selectedShortRemarks = await AppServices.Current.VisitShortRemarkService.GetAllByVisitId(OngoingDto.Visit.Id);
                if (selectedShortRemarks != null && selectedShortRemarks.Any())
                    OngoingDto.SelectedShortRemarks = BaseAssembler.BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(i => selectedShortRemarks.Select(x => x.ShortRemarkId).Contains(i.Id)).ToList());
                else OngoingDto.SelectedShortRemarks = new ObservableCollection<string>();

                var selectedHealthStatuses = await AppServices.Current.VisitHealthStatusService.GetAllByVisitId(OngoingDto.Visit.Id);
                if (selectedHealthStatuses != null && selectedHealthStatuses.Any())
                    OngoingDto.SelectedHealthStatuses = BaseAssembler.BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(i => selectedHealthStatuses.Select(x => x.HealthStatusId).Contains(i.Id)).ToList());
                else OngoingDto.SelectedHealthStatuses = new ObservableCollection<string>();

                ShowStartVisitButton = OngoingDto.Visit.VisitStatusId == AppData.Current.Codes.FirstOrDefault(x =>
                    x.Type == ECodeType.VISIT_STATUS.ToString() && x.Name == ECodeName.ACKNOWLEDGED.ToString()).Id;
                OngoingDto.HandOverNotesEntered = OngoingDto.Visit.HandOverNotes;
            }
            else
            {
                ShowStartVisitButton = true;
                OngoingDto.HandOverNotesEntered = null;
            }

            Title = OngoingDto != null && !ShowStartVisitButton ? "Ongoing Booking" : "Upcoming Booking";
            var TabTitle = OngoingDto != null && !ShowStartVisitButton ? "Ongoing" : "Upcoming";
            WeakReferenceMessenger.Default.Send(new MessagingEvents.PageTabTitleChangeMessage(TabTitle));
            OngoingDto.MaxCharRemainingText = MaxChars - (string.IsNullOrEmpty(OngoingDto.HandOverNotesEntered) ? 0 : OngoingDto.HandOverNotesEntered.Length) + " character(s) remaining";

        }
        else
        {
            OngoingDto = null;
            RefreshOnAppear = true;
        }
    }

    private async Task StartVisit()
    {
        var now = DateTimeExtensions.NowNoTimezone();
        var errors = await App.FeaturePermissionHandlerService.CheckEverything();
        if (errors != null && errors.Any())
        {
            var isTampered = errors.Any(x => x.Type == EFeaturePermissionOrderedType.ClockTampered);
            if (isTampered && AppServices.Current.CareWorkerTrackerService != null && AppServices.Current.CareWorkerTrackerService.OnGoingBooking != null)
            {
                AppServices.Current.CareWorkerTrackerService.Visit.IsVisitTampered = true;
                await AppServices.Current.VisitService.SyncVisit(AppServices.Current.CareWorkerTrackerService.Visit);
            }

            await OpenActions.OpenErrorPage();
            return;
        }

        if (App.IsFingerprinting)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.VisitDuringFingerprint, false);
            return;
        }

        var currentDevicelocation = await GeolocationExtensions.GetBestLocationAsync();
        if (currentDevicelocation.IsFromMockProvider)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.MockLocationDetected, false);
            return;
        }

        /*
         * TODO: REMOVE_BYPASS
        if (now < OngoingDto.Booking.StartTime.AddMinutes(-Constants.StartVisitNextBookingThresholdInMins)
                || now > OngoingDto.Booking.EndTime)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.NextBookingThresholdError, false);
            return;
        }
        */

        var biometricDto = await SystemHelper.Current.VerifyBiometric();
        if (biometricDto == null)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.BiometricVerificationFailure, false);
            return;
        }

        var startVisitMessage = await AppServices.Current.VisitService.CanStartVisit(new StartVisitDto
        {
            BookingDetailId = OngoingDto.BookingDetail.Id,
            DeviceInfo = DeviceInfo.Platform.ToString(),
            Latitude = currentDevicelocation.Latitude.ToString(),
            Longitude = currentDevicelocation.Longitude.ToString()
        });
        if (string.IsNullOrEmpty(startVisitMessage))
        {
            if (!OngoingDto.IsMaster)
                Preferences.Default.Set(Constants.PrefKeyOngoingNonMasterCwBookingId, OngoingDto.Booking.Id);

            OngoingDto.Booking.HandOverNotes = await AppServices.Current.BookingService.UpdateHandOverNotes(OngoingDto.Booking.Id);

            if (AppServices.Current.CareWorkerTrackerService.OnGoingBooking != null && AppServices.Current.CareWorkerTrackerService.OnGoingBookingDetailId > 0
                && AppServices.Current.CareWorkerTrackerService.OnGoingBookingDetailId != OngoingDto.BookingDetail.Id)
                await AppServices.Current.CareWorkerTrackerService.StopNormalMode(ESensorExitReason.NEXT_VISIT_START);

            OngoingDto.Visit = await AppServices.Current.CareWorkerTrackerService.StartNormalModeBySV(OngoingDto.BookingDetail.Id, biometricDto);
            Preferences.Default.Set(Constants.PrefKeyShowBookingIncomplete, OngoingDto.Booking.Id);
        }
        else await App.Current.MainPage.ShowSnackbar(startVisitMessage, false);
    }

    private async Task OnSubmit(bool isSuccess = false)
    {
        var now = DateTimeExtensions.NowNoTimezone();
        var errors = await App.FeaturePermissionHandlerService.CheckEverything();
        if (errors != null && errors.Any())
        {
            var isTampered = errors.Any(x => x.Type == EFeaturePermissionOrderedType.ClockTampered);
            if (isTampered && AppServices.Current.CareWorkerTrackerService != null && AppServices.Current.CareWorkerTrackerService.OnGoingBooking != null)
            {
                AppServices.Current.CareWorkerTrackerService.Visit.IsVisitTampered = true;
                await AppServices.Current.VisitService.SyncVisit(AppServices.Current.CareWorkerTrackerService.Visit);
            }

            await OpenActions.OpenErrorPage();
            return;
        }

        if (OngoingDto.IsMaster)
        {
            var visitTasksToCheck = await AppServices.Current.VisitTaskService.GetAllByVisitId(OngoingDto.Visit.Id);
            if (OngoingDto.TaskList.Count != visitTasksToCheck.Count || visitTasksToCheck.Any(x => x.CompletedOn == null))
            {
                await App.Current.MainPage.ShowSnackbar(Messages.TaskCompletionBeforeSubmit, false);
                return;
            }

            var visitMedicationsToCheck = await AppServices.Current.VisitMedicationService.GetAllByVisitId(OngoingDto.Visit.Id);
            if (OngoingDto.MedicationList.Count != visitMedicationsToCheck.Count || visitMedicationsToCheck.Any(x => x.CompletedOn == null))
            {
                await App.Current.MainPage.ShowSnackbar(Messages.MedicationCompletionBeforeSubmit, false);
                return;
            }

            if (OngoingDto.SelectedHealthStatuses == null || !OngoingDto.SelectedHealthStatuses.Any())
            {
                await App.Current.MainPage.ShowSnackbar(Messages.HealthStatusRequired, false);
                return;
            }

            if (OngoingDto.SelectedShortRemarks == null || !OngoingDto.SelectedShortRemarks.Any())
            {
                await App.Current.MainPage.ShowSnackbar(Messages.ShortRemarksRequired, false);
                return;
            }
        }

        var currentDevicelocation = await GeolocationExtensions.GetBestLocationAsync();
        if (currentDevicelocation.IsFromMockProvider)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.MockLocationDetected, false);
            return;
        }

        var biometricDto = await SystemHelper.Current.VerifyBiometric();
        if (biometricDto == null)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.BiometricVerificationFailure, false);
            return;
        }

        if (OngoingDto.BookingDetails.Count > 1)
        {
            var canSubmitReport = await AppServices.Current.VisitService.CanSubmitReport(OngoingDto.Booking.Id);
            if (!canSubmitReport)
            {
                await App.Current.MainPage.ShowSnackbar(OngoingDto.BookingDetail.IsMaster ?
                    Messages.UnableToEndVisitCwMissing : Messages.UnableToEndVisitReportMissing, false);
                return;
            }
        }

        OngoingDto.Visit.CompletedOn = now;
        OngoingDto.Visit.IsCompleted = true;
        OngoingDto.Visit.HandOverNotes = OngoingDto.HandOverNotesEntered;
        OngoingDto.Visit.Summary = OngoingDto.Summary;
        OngoingDto.Visit.VisitStatusId = AppData.Current.Codes.FirstOrDefault(x => x.Type == ECodeType.VISIT_STATUS.ToString() && x.Name == ECodeName.COMPLETED.ToString()).Id;

        if (currentDevicelocation != null)
            OngoingDto.Visit.UploadedLocation = currentDevicelocation.Latitude + "," + currentDevicelocation.Longitude;

        //Check whether the ongoing notification service can be stopped if app not running on background
        if (AppServices.Current.CareWorkerTrackerService.ExitedGeozone.HasValue && AppServices.Current.CareWorkerTrackerService.ExitedGeozone.Value)
        {
            OngoingDto.Visit.TerminationReason = ESensorExitReason.UPLOAD_OUTSIDE_EXIT.ToString();
            OngoingDto.Visit.TerminatedOn = now;
        }

        OngoingDto.Visit = await AppServices.Current.VisitService.InsertOrReplace(OngoingDto.Visit);
        AppServices.Current.CareWorkerTrackerService.Visit = OngoingDto.Visit;

        var currentBooking = await AppServices.Current.BookingService.SetCurrentBookingStatus(OngoingDto.Booking.Id,
                                ECodeType.BOOKING_STATUS, ECodeName.PROGRESS);

        //To send it to server as completed booking but keep in mobile as progress until uploaded
        var bookingCompletedCode = await AppServices.Current.CodeService.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.COMPLETED);
        currentBooking.CompletionStatusId = bookingCompletedCode.Id;
        currentBooking.CompletedOn = now;
        currentBooking.IsCompleted = true;
        currentBooking = await AppServices.Current.BookingService.InsertOrReplace(currentBooking);
        AppServices.Current.CareWorkerTrackerService.OnGoingBooking = currentBooking;

        List<BodyMap> bodyMaps = null;
        List<Attachment> attachments = null;
        List<VisitShortRemark> shortRemarks = null;
        List<VisitHealthStatus> healthStatuses = null;
        IList<VisitTask> visitTasks = null;
        IList<VisitMedication> visitMedications = null;
        VisitFluid fluid = null;
        IList<Incident> incidents = null;
        List<VisitConsumable> consumables = null;

        if (OngoingDto.IsMaster)
        {
            await AppServices.Current.VisitHealthStatusService.DeleteAllByVisitId(OngoingDto.Visit.Id);
            await AppServices.Current.VisitShortRemarkService.DeleteAllByVisitId(OngoingDto.Visit.Id);
            await AppServices.Current.VisitConsumableService.DeleteAllByVisitId(OngoingDto.Visit.Id);

            foreach (var healthStatus in OngoingDto.SelectedHealthStatuses)
                await AppServices.Current.VisitHealthStatusService.InsertOrReplace(new VisitHealthStatus
                {
                    VisitId = OngoingDto.Visit.Id,
                    HealthStatusId = AppData.Current.VisitMessages.FirstOrDefault(x => x.Message == healthStatus).Id
                });

            foreach (var shortRemark in OngoingDto.SelectedShortRemarks)
                await AppServices.Current.VisitShortRemarkService.InsertOrReplace(new VisitShortRemark
                {
                    VisitId = OngoingDto.Visit.Id,
                    ShortRemarkId = AppData.Current.VisitMessages.FirstOrDefault(x => x.Message == shortRemark).Id
                });

            var consumablesToPersist = OngoingDto.ConsumableList.Where(x => x.QuantityUsed.HasValue && x.QuantityUsed.Value > 0).ToList();
            if (consumablesToPersist != null && consumablesToPersist.Any())
                foreach (var consumable in consumablesToPersist)
                    await AppServices.Current.VisitConsumableService.InsertOrReplace(new VisitConsumable
                    {
                        ConsumableTypeId = consumable.ConsumableTypeId,
                        ConsumableTypeStr = consumable.ConsumableTypeStr,
                        QuantityUsed = consumable.QuantityUsed,
                        VisitId = OngoingDto.Visit.Id,
                    });

            bodyMaps = (await AppServices.Current.BodyMapService.GetAllByBaseVisitId(OngoingDto.Visit.Id))?.ToList();
            attachments = (await AppServices.Current.AttachmentService.GetAllByBaseVisitId(OngoingDto.Visit.Id))?.ToList();
            shortRemarks = (await AppServices.Current.VisitShortRemarkService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();
            healthStatuses = (await AppServices.Current.VisitHealthStatusService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();
            consumables = (await AppServices.Current.VisitConsumableService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();

            visitTasks = await AppServices.Current.VisitTaskService.GetAllByVisitId(OngoingDto.Visit.Id);
            visitMedications = await AppServices.Current.VisitMedicationService.GetAllByVisitId(OngoingDto.Visit.Id);
            fluid = await AppServices.Current.VisitFluidService.GetByVisitId(OngoingDto.Visit.Id);
            incidents = await AppServices.Current.IncidentService.GetAllByVisitId(OngoingDto.Visit.Id);

            if (attachments != null && attachments.Any())
                await AttachmentHelper.Current.UploadFiles(attachments,
                    string.Join('-', [currentBooking.StartTime.ToString("yyyy"), currentBooking.StartTime.ToString("MM")]), currentBooking.BookingCode);
        }

        var locationList = await AppServices.Current.LocationService.GetAllByVisitId(OngoingDto.Visit.Id);
        var stepsList = await AppServices.Current.StepCountService.GetAllByVisitId(OngoingDto.Visit.Id);

        var isSubmitSuccess = await AppServices.Current.VisitService.SubmitVisitReport(new VisitReportDto()
        {
            BookingDetailId = OngoingDto.BookingDetail.Id,
            DoPostProcessing = OngoingDto.Visit.TerminationReason == ESensorExitReason.NO_UPLOAD_SV_TIMEOUT.ToString()
                                ? true : (AppServices.Current.CareWorkerTrackerService?.ExitedGeozone ?? false),
            Visit = OngoingDto.Visit,
            VisitBiometric = biometricDto,
            VisitTaskList = visitTasks,
            VisitMedicationList = visitMedications,
            VisitFluid = fluid,
            IncidentList = incidents,
            BodyMapList = bodyMaps,
            AttachmentList = attachments,
            ConsumableList = consumables,
            ShortRemarkList = shortRemarks,
            HealthStatusList = healthStatuses,
            LocationList = locationList,
            StepCountList = stepsList
        });

        await App.Current.MainPage.ShowSnackbar(
            isSubmitSuccess ? Messages.UploadSuccess : Messages.UploadFailure,
            isSubmitSuccess, !isSubmitSuccess);

        if (isSubmitSuccess)
        {
            await AppServices.Current.VisitService.DeleteVisitsByBookingId(OngoingDto.Booking.Id);
            await AppServices.Current.BookingService.DownloadVisitsByBookingId(OngoingDto.Booking.Id);

            if (AppServices.Current.CareWorkerTrackerService.ExitedGeozone.HasValue && AppServices.Current.CareWorkerTrackerService.ExitedGeozone.Value)
                await AppServices.Current.CareWorkerTrackerService.StopNormalMode(ESensorExitReason.UPLOAD_OUTSIDE_EXIT);

            var cwDashboardVm = ServiceLocator.GetService<CareWorkerDashboardVm>();
            if (cwDashboardVm != null) cwDashboardVm.RefreshOnAppear = true;
            var bookingsVm = ServiceLocator.GetService<BookingsVm>();
            if (bookingsVm != null) bookingsVm.RefreshOnAppear = true;

            if (!OngoingDto.IsMaster) Preferences.Default.Remove(Constants.PrefKeyOngoingNonMasterCwBookingId);

            await InitCommand.Execute(true);
        }
    }

    private async Task DownloadHandOverNotes()
    {
        OngoingDto.HandOverNotes = await AppServices.Current.BookingService.UpdateHandOverNotes(OngoingDto.Booking.Id);
    }

    protected async Task CheckMasterCwChange()
    {
        var editAllowed = await AppServices.Current.BookingService.CheckMasterCwChange(OngoingDto.BookingDetail.Id);
        await App.Current.MainPage.DisplayAlert(Messages.Message,
            editAllowed.HasValue && editAllowed.Value
            ? Messages.BookingEditAllowed : Messages.BookingEditNotAllowed, Messages.Ok);

        if (editAllowed.HasValue && editAllowed.Value)
        {
            var bookingDetails = await AppServices.Current.BookingDetailService.GetAllForBooking(OngoingDto.Booking.Id);
            foreach (var bookingDetail in bookingDetails)
                bookingDetail.IsMaster = bookingDetail.CareWorkerId == AppData.Current.CurrentProfile.Id
                    && bookingDetail.Id == OngoingDto.BookingDetail.Id;
            await AppServices.Current.BookingDetailService.InsertOrReplace(bookingDetails);

            await InitCommand.Execute(true);
        }
    }

    protected async Task OpenTask(int taskId)
    {
        await Shell.Current.Navigation.PopToRootAsync();

        var navigationParameter = new Dictionary<string, object>
            {
                { "Id", taskId },
                { "BaseVisitId", OngoingDto.Visit.Id },
                { "IsReadOnly", !OngoingDto.IsMaster }
            };
        var taskDetailViewPage = $"{nameof(TaskDetailPage)}?";
        await Shell.Current.GoToAsync(taskDetailViewPage, navigationParameter);
    }

    protected async Task OpenMedication(int medicationId)
    {
        await Shell.Current.Navigation.PopToRootAsync();

        var navigationParameter = new Dictionary<string, object>
            {
                { "Id", medicationId },
                { "BaseVisitId", OngoingDto.Visit.Id },
                { "IsReadOnly", !OngoingDto.IsMaster }
            };
        var medicationDetailViewPage = $"{nameof(MedicationDetailPage)}?";
        await Shell.Current.GoToAsync(medicationDetailViewPage, navigationParameter);
    }

    protected async Task OpenFluidDetail(int fluidId)
    {
        await Shell.Current.Navigation.PopToRootAsync();

        var navigationParameter = new Dictionary<string, object>
            {
                { "Id", fluidId },
                { "BaseVisitId", OngoingDto.Visit.Id },
                { "BookingId", Booking.Id },
                { "IsReadOnly", !OngoingDto.IsMaster }
            };
        var fluidDetailPage = $"{nameof(FluidDetailPage)}?";
        await Shell.Current.GoToAsync(fluidDetailPage, navigationParameter);
    }

    private async Task OnVisitStart((bool, int) value)
    {
        if (value.Item1)
        {
            var startedBookingDetail = await AppServices.Current.BookingDetailService.GetById(value.Item2);
            var bookingDetails = await AppServices.Current.BookingDetailService.GetAllForBooking(startedBookingDetail.BookingId);
            foreach (var bookingDetail in bookingDetails)
            {
                var bd = await AppServices.Current.BookingDetailService.GetById(bookingDetail.Id);
                bd.Eta = null;
                bd.EtaStatusText = null;
                bd.EtaStatusColor = null;
                bd.EtaOn = null;
                bd.EtaAvailable = false;
                await AppServices.Current.BookingDetailService.InsertOrReplace(bd);
            }

            if (AppData.Current.CurrentProfile.Type == nameof(EUserType.CAREWORKER))
            {
                var cwDashboardVm = ServiceLocator.GetService<CareWorkerDashboardVm>();
                if (cwDashboardVm != null) cwDashboardVm.RefreshOnAppear = true;

                var bookingsVm = ServiceLocator.GetService<BookingsVm>();
                if (bookingsVm != null) bookingsVm.RefreshOnAppear = true;
            }
        }

        await InitCommand.Execute(true);
    }

    public void SubscribeAllMessagingCenters()
    {
        var isBodyMapChangeRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.BookingPageBodyMapChangedMessage>(this);
        if (!isBodyMapChangeRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.BookingPageBodyMapChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (OngoingDto?.Visit != null && message.Value == OngoingDto.Visit.Id)
                    {
                        var bodyMaps = await AppServices.Current.BodyMapService.GetAllByVisitId(OngoingDto.Visit.Id);
                        OngoingDto.BodyMapList = bodyMaps.OrderBy(i => i.AddedOn).ToList();
                    }
                });

        var isIncidentChangeRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.BookingPageIncidentReportChangedMessage>(this);
        if (!isIncidentChangeRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.BookingPageIncidentReportChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (OngoingDto?.Visit != null && message.Value == OngoingDto.Visit.Id)
                    {
                        var incidents = await AppServices.Current.IncidentService.GetAllByVisitId(OngoingDto.Visit.Id);
                        OngoingDto.IncidentList = incidents.OrderBy(i => i.CompletedOn)
                                    .Select((incident, index) =>
                                    {
                                        incident.DisplayName = $"IR-{index + 1}";
                                        return incident;
                                    }).ToList();
                    }
                });

        var isTaskRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.BookingPageTaskChangedMessage>(this);
        if (!isTaskRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.BookingPageTaskChangedMessage>(this,
                async (recipient, message) =>
                    {
                    if (OngoingDto?.Visit != null && message.Value == OngoingDto.Visit.Id)
                        {
                            var tasks = await AppServices.Current.TaskService.GetAllByBookingId(Booking.Id);
                            OngoingDto.TaskList = await BaseAssembler.BuildTaskViewModels(tasks, OngoingDto.Visit.Id);
                        }
                    });

        var isMedicationRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.BookingPageMedicationChangedMessage>(this);
        if (!isMedicationRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.BookingPageMedicationChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (OngoingDto?.Visit != null && message.Value == OngoingDto.Visit.Id)
                    {
                        var medications = await AppServices.Current.MedicationService.GetAllByBookingId(Booking.Id);
                        OngoingDto.MedicationList = await BaseAssembler.BuildMedicationsViewModels(medications, OngoingDto.Visit.Id);
                    }
                });

        var isFluidRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.BookingPageFluidChangedMessage>(this);
        if (!isFluidRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.BookingPageFluidChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (OngoingDto?.Visit != null && message.Value == OngoingDto.Visit.Id)
                    {
                        var fluid = await AppServices.Current.VisitFluidService.GetByVisitId(OngoingDto.Visit.Id);
                        OngoingDto.Fluid = BaseAssembler.BuildFluidViewModels(fluid);
                    }
                });

        var isStartedRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.CareWorkerVisitStartedMessage>(this);
        if (!isStartedRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.CareWorkerVisitStartedMessage>(this,
                async (recipient, message) => await OnVisitStart(message.Value));

        var isCompletedRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.VisitCompletedMessage>(this);
        if (!isCompletedRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.VisitCompletedMessage>(this,
                async (recipient, message) => await InitCommand.Execute(true));

        var isPreVisitMonitorRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.PreVisitMonitorMessage>(this);
        if (!isPreVisitMonitorRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.PreVisitMonitorMessage>(this,
                async (recipient, message) =>
                {
                    if (message.Value && ShowStartVisitButton)
                        await InitCommand.Execute(true);
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.CareWorkerVisitStartedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.VisitCompletedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.PreVisitMonitorMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageBodyMapChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageIncidentReportChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageTaskChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageMedicationChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageFluidChangedMessage>(this);
    }
}