namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
[QueryProperty(nameof(VisitId), nameof(VisitId))]
public class BookingEditVm : BaseVm, IQueryAttributable, IDisposable
{
    public int Id { get; set; }
    public int VisitId { get; set; }

    public static int MaxChars = 2000;

    public bool ShowHandOverNotesButton { get; set; } = true;
    public string HandOverNotes { get; set; }

    public OngoingDto OngoingDto { get; set; }
    public Booking Booking { get; set; }

    public string TypeName => EPageType.BookingType.ToString();

    public ReactiveCommand<Unit, Unit> OnBackCommand { get; }
    public ReactiveCommand<bool, Unit> SubmitCommand { get; }
    public ReactiveCommand<int, Unit> OpenTaskCommand { get; }
    public ReactiveCommand<int, Unit> OpenFluidDetailCommand { get; }
    public ReactiveCommand<int, Unit> OpenMedicationCommand { get; }
    public ReactiveCommand<Unit, Unit> DownloadHandOverNotesCommand { get; }

    private bool _isHandOverNotesExpanded;
    private bool _hasHandOverNoteDownloaded;

    public bool IsHandOverNotesExpanded
    {
        get => _isHandOverNotesExpanded;
        set
        {
            this.RaiseAndSetIfChanged(ref _isHandOverNotesExpanded, value);
            if (_isHandOverNotesExpanded && !_hasHandOverNoteDownloaded)
            {
                _hasHandOverNoteDownloaded = true;
                DownloadHandOverNotesCommand?.Execute(Unit.Default);
            }
        }
    }

    public BookingEditVm()
    {
        OnBackCommand = ReactiveCommand.CreateFromTask(OnBack);
        SubmitCommand = ReactiveCommand.CreateFromTask<bool>(OnSubmit);
        OpenTaskCommand = ReactiveCommand.CreateFromTask<int>(OpenTask);
        OpenFluidDetailCommand = ReactiveCommand.CreateFromTask<int>(OpenFluidIntake);
        OpenMedicationCommand = ReactiveCommand.CreateFromTask<int>(OpenMedication);
        DownloadHandOverNotesCommand = ReactiveCommand.CreateFromTask(DownloadHandOverNotes);

        BindBusyWithException(OnBackCommand);
        BindBusyWithException(SubmitCommand);
        BindBusyWithException(OpenTaskCommand);
        BindBusyWithException(OpenFluidDetailCommand);
        BindBusyWithException(OpenMedicationCommand);
        BindBusyWithException(DownloadHandOverNotesCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        Booking = await AppServices.Current.BookingService.GetById(Id);
        var visitDto = await AppServices.Current.VisitService.GetVisitDtoByBookingId(Id);
        Preferences.Default.Set(Constants.PrefKeyEditBookingId, Id);
        Preferences.Default.Set(Constants.PrefKeyEditBookingVisitContent, JsonExtensions.Serialize(visitDto));

        IsReadOnly = false;

        if (Booking == null)
        {
            await AppServices.Current.BookingService.SyncBookingById(Id);
            Booking = await AppServices.Current.BookingService.GetById(Id);
        }

        Title = "Edit Booking";

        var Visit = await AppServices.Current.VisitService.GetById(VisitId);
        OngoingDto = await BaseAssembler.BuildOngoingViewModels(Booking, Visit);

        if (AppData.Current.VisitMessages != null && AppData.Current.VisitMessages.Any())
        {
            OngoingDto.ShortRemarks = BaseAssembler.BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(i => i.Type == EMessageType.SHORT_REMARKS.ToString()).ToList());
            OngoingDto.HealthStatuses = BaseAssembler.BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(i => i.Type == EMessageType.HEALTH_STATUS.ToString()).ToList());
        }

        var shortRemarks = await AppServices.Current.VisitShortRemarkService.GetAllByVisitId(OngoingDto.Visit.Id);
        if (shortRemarks != null && shortRemarks.Any())
            OngoingDto.SelectedShortRemarks = BaseAssembler.BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(i => shortRemarks.Select(x => x.ShortRemarkId).Contains(i.Id)).ToList());
        else OngoingDto.SelectedShortRemarks = new ObservableCollection<string>();

        var healthStatuses = await AppServices.Current.VisitHealthStatusService.GetAllByVisitId(OngoingDto.Visit.Id);
        if (healthStatuses != null && healthStatuses.Any())
            OngoingDto.SelectedHealthStatuses = BaseAssembler.BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(i => healthStatuses.Select(x => x.HealthStatusId).Contains(i.Id)).ToList());
        else OngoingDto.SelectedHealthStatuses = new ObservableCollection<string>();

        OngoingDto.HandOverNotesEntered = OngoingDto.Visit.HandOverNotes;
    }

    private async Task OnSubmit(bool isSuccess = false)
    {
        var isTampered = await AppServices.Current.TamperingService.IsTimeTampered();
        if (isTampered)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.DateTimeTampered, false, true);
            return;
        }

        var now = DateTimeExtensions.NowNoTimezone();
        var currentDevicelocation = await GeolocationExtensions.GetBestLocationAsync();
        if (currentDevicelocation.IsFromMockProvider)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.MockLocationDetected, false);
            return;
        }

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

        OngoingDto.Visit.CompletedOn = now;
        OngoingDto.Visit.IsCompleted = true;
        OngoingDto.Visit.HandOverNotes = OngoingDto.HandOverNotesEntered;
        OngoingDto.Visit.Summary = OngoingDto.Summary;

        OngoingDto.Visit.VisitStatusId = AppData.Current.Codes.FirstOrDefault(x => x.Type == ECodeType.VISIT_STATUS.ToString() && x.Name == ECodeName.COMPLETED.ToString()).Id;

        if (currentDevicelocation != null)
            OngoingDto.Visit.UploadedLocation = currentDevicelocation.Latitude + "," + currentDevicelocation.Longitude;

        OngoingDto.Visit = await AppServices.Current.VisitService.InsertOrReplace(OngoingDto.Visit);

        var currentBooking = await AppServices.Current.BookingService.SetCurrentBookingStatus(OngoingDto.Booking.Id,
                                ECodeType.BOOKING_STATUS, ECodeName.PROGRESS);

        //To send it to server as completed booking but keep in mobile as progress until uploaded
        var bookingCompletedCode = await AppServices.Current.CodeService.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.COMPLETED);
        currentBooking.CompletionStatusId = bookingCompletedCode.Id;
        currentBooking.CompletedOn = now;
        currentBooking.IsCompleted = true;
        currentBooking = await AppServices.Current.BookingService.InsertOrReplace(currentBooking);

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

        var bodyMaps = (await AppServices.Current.BodyMapService.GetAllByBaseVisitId(OngoingDto.Visit.Id))?.ToList();
        var attachments = (await AppServices.Current.AttachmentService.GetAllByBaseVisitId(OngoingDto.Visit.Id))?.ToList();
        var shortRemarks = (await AppServices.Current.VisitShortRemarkService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();
        var healthStatuses = (await AppServices.Current.VisitHealthStatusService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();
        var consumables = (await AppServices.Current.VisitConsumableService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();

        var visitTasks = (await AppServices.Current.VisitTaskService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();
        var visitMedications = (await AppServices.Current.VisitMedicationService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();
        var fluid = await AppServices.Current.VisitFluidService.GetByVisitId(OngoingDto.Visit.Id);
        var incidents = (await AppServices.Current.IncidentService.GetAllByVisitId(OngoingDto.Visit.Id))?.ToList();

        if (attachments != null && attachments.Any())
            await AttachmentHelper.Current.UploadFiles(attachments.Where(x => x.ServerRef == default).ToList(),
                string.Join('-', new[] { currentBooking.StartTime.ToString("yyyy"), currentBooking.StartTime.ToString("MM") }), currentBooking.BookingCode);

        var isSubmitSuccess = await AppServices.Current.VisitService.SubmitVisitEditReport(new VisitReportEditDto()
        {
            BookingDetailId = OngoingDto.BookingDetail.Id,
            Visit = OngoingDto.Visit,
            VisitTaskList = visitTasks,
            VisitMedicationList = visitMedications,
            VisitFluid = fluid,
            IncidentList = incidents,
            BodyMapList = bodyMaps,
            AttachmentList = attachments,
            ConsumableList = consumables,
            ShortRemarkList = shortRemarks,
            HealthStatusList = healthStatuses
        });

        await App.Current.MainPage.ShowSnackbar(
            isSubmitSuccess ? Messages.UploadSuccess : Messages.UploadFailure,
            isSubmitSuccess, !isSubmitSuccess);

        if (isSubmitSuccess)
        {
            await AppServices.Current.VisitService.DeleteVisitsByBookingId(OngoingDto.Booking.Id);
            await AppServices.Current.BookingService.DownloadVisitsByBookingId(OngoingDto.Booking.Id);

            var bookingsVm = ServiceLocator.GetService<BookingsVm>();
            if (bookingsVm != null) bookingsVm.RefreshOnAppear = true;

            if (Shell.Current.Navigation.NavigationStack != null)
            {
                var serviceUserDetailVm = nameof(ServiceUserDetailPage);
                if (Shell.Current.Navigation.NavigationStack.Any(x => x != null && x.GetType().Name == serviceUserDetailVm))
                    WeakReferenceMessenger.Default.Send(new MessagingEvents.ServiceUserDetailPageBookingEditCompleted(true));

                var careWorkerDetailVm = nameof(CareWorkerDetailPage);
                if (Shell.Current.Navigation.NavigationStack.Any(x => x != null && x.GetType().Name == careWorkerDetailVm))
                    WeakReferenceMessenger.Default.Send(new MessagingEvents.CareWorkerDetailPageBookingEditCompleted(true));
            }

            Preferences.Default.Remove(Constants.PrefKeyEditBookingId);
            Preferences.Default.Remove(Constants.PrefKeyEditBookingVisitContent);

            await Shell.Current.Navigation.PopAsync();
        }
    }

    private new async Task OnBack()
    {
        bool answer = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle, Messages.DialogExitWithoutUploading, Messages.Yes, Messages.No);
        if (answer) await AppServices.Current.DataRetentionService.RemoveIncompleteBookingEdit();

        await base.OnBack();
    }

    private async Task DownloadHandOverNotes()
    {
        OngoingDto.HandOverNotes = await AppServices.Current.BookingService.UpdateHandOverNotes(OngoingDto.Booking.Id);
        ShowHandOverNotesButton = false;
    }

    protected async Task OpenTask(int taskId)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Id", taskId },
            { "BaseVisitId", OngoingDto.Visit.Id },
            { "IsReadOnly", false}
        };
        var taskDetailViewPage = $"{nameof(TaskDetailPage)}?";
        await Shell.Current.GoToAsync(taskDetailViewPage, navigationParameter);
    }

    protected async Task OpenMedication(int medicationId)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Id", medicationId },
            { "BaseVisitId", OngoingDto.Visit.Id },
            { "IsReadOnly", false}
        };
        var medicationDetailViewPage = $"{nameof(MedicationDetailPage)}?";
        await Shell.Current.GoToAsync(medicationDetailViewPage, navigationParameter);
    }

    protected async Task OpenFluidIntake(int fluidId)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "Id", fluidId },
            { "BaseVisitId", OngoingDto.Visit.Id },
            { "BookingId", Booking.Id },
            { "IsReadOnly", false}
        };
        var fluidDetailPage = $"{nameof(FluidDetailPage)}?";
        await Shell.Current.GoToAsync(fluidDetailPage, navigationParameter);
    }

    public void SubscribeAllMessagingCenters()
    {
        var isBodyMapChangeRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.BookingPageBodyMapChangedMessage>(this);
        if (!isBodyMapChangeRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.BookingPageBodyMapChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (message.Value == VisitId)
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
                    if (message.Value == VisitId)
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
                        if (message.Value == VisitId)
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
                    if (message.Value == VisitId)
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
                    if (message.Value == VisitId)
                    {
                        var fluid = await AppServices.Current.VisitFluidService.GetByVisitId(OngoingDto.Visit.Id);
                        OngoingDto.Fluid = BaseAssembler.BuildFluidViewModels(fluid);
                    }
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageBodyMapChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageIncidentReportChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageTaskChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageMedicationChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.BookingPageFluidChangedMessage>(this);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var id))
            Id = Convert.ToInt32(id.ToString());
        if (query.TryGetValue(nameof(VisitId), out var visitId))
            VisitId = Convert.ToInt32(visitId.ToString());

        await InitCommand.Execute();
    }
}