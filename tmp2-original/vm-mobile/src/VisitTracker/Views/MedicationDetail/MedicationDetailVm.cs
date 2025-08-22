namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
[QueryProperty(nameof(BaseVisitId), nameof(BaseVisitId))]
[QueryProperty(nameof(IsReadOnly), nameof(IsReadOnly))]
public class MedicationDetailVm : BaseVm, IQueryAttributable, IDisposable
{
    public int Id { get; set; }
    public int BaseVisitId { get; set; }

    public BookingMedication Medication { get; set; }
    public VisitMedication VisitMedication { get; set; }
    public Booking Booking { get; set; }
    public ServiceUser ServiceUser { get; set; }
    public UserCardDto ServiceUserCard { get; set; }
    public FormattedString AllTimeSlotsStr { get; set; }
    public FormattedString AllDaysStr { get; set; }
    public IList<AttachmentDto> MedicationImages { get; set; }
    public IList<Code> StatusList { get; set; }
    public IList<Code> StatusDetailList { get; set; }
    public IList<BodyMap> BodyMapList { get; set; }
    public IList<Incident> IncidentList { get; set; }
    public Code SelectedStatus { get; set; }
    public Code SelectedDetailStatus { get; set; }

    public string TypeName => EPageType.MedicationType.ToString();

    public IList<AttachmentDto> AttachmentList { get; set; }
    public bool ShowCompletionStatus { get; set; }
    public bool EnableCompletionStatus { get; set; }
    public bool IsEnquireOfficeVisible { get; set; }
    public EMedicationAdministerAction MedicationActionMessage { get; set; }
    public bool IsMedicationActionMessageVisible { get; set; }

    public ReactiveCommand<Code, Unit> StatusChangedCommand { get; }
    public ReactiveCommand<bool, Unit> SubmitCommand { get; }
    public ReactiveCommand<Unit, Unit> OnReqMedicationAdministrationCommand { get; }
    public ReactiveCommand<Unit, Unit> OnBackCommand { get; }

    public MedicationDetailVm()
    {
        StatusChangedCommand = ReactiveCommand.CreateFromTask<Code>(StatusChanged);
        SubmitCommand = ReactiveCommand.CreateFromTask<bool>(OnSubmit);
        OnReqMedicationAdministrationCommand = ReactiveCommand.CreateFromTask(OnReqMedicationAdministration);
        OnBackCommand = ReactiveCommand.CreateFromTask(OnBack);

        BindBusyWithException(SubmitCommand);
        BindBusyWithException(OnReqMedicationAdministrationCommand);
        BindBusyWithException(OnBackCommand);
        BindBusyWithException(StatusChangedCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        Medication = await AppServices.Current.MedicationService.GetById(Id);
        VisitMedication = await AppServices.Current.VisitMedicationService.GetByMedicationAndVisitId(Medication.Id, BaseVisitId);
        if (VisitMedication == null)
        {
            VisitMedication = new VisitMedication();
            VisitMedication.MedicationId = Medication.Id;
            VisitMedication.VisitId = BaseVisitId;
            VisitMedication = await AppServices.Current.VisitMedicationService.InsertOrReplace(VisitMedication);
        }

        BodyMapList = (await AppServices.Current.BodyMapService.GetAllByVisitMedicationId(VisitMedication.Id)).ToList();
        var attachments = await AppServices.Current.AttachmentService.GetAllByVisitMedicationId(VisitMedication.Id);
        AttachmentList = BaseAssembler.BuildAttachmentViewModels(attachments);

        Booking = await AppServices.Current.BookingService.GetById(Medication.BookingId);
        ServiceUser = await AppServices.Current.ServiceUserService.GetById(Booking.ServiceUserId);
        ServiceUserCard = await BaseAssembler.BuildUserCardFromSu(ServiceUser, Booking);

        if (!string.IsNullOrEmpty(Medication?.ImageUrl))
            MedicationImages = new List<AttachmentDto> { new AttachmentDto { S3Url = Medication.ImageUrl } };

        StatusList = AppData.Current.Codes?.Where(x => x.Type == ECodeType.MEDICATION_COMPLETION.ToString()).OrderBy(i => i.Order).ToList();
        SelectedStatus = StatusList.FirstOrDefault(I => I.Id == VisitMedication.CompletionStatusId);
        if (SelectedStatus != null)
        {
            if (VisitMedication.CompletionStatusDetailId != null)
            {
                SelectedDetailStatus = AppData.Current.Codes.FirstOrDefault(x => x.Id == VisitMedication.CompletionStatusDetailId);
                StatusDetailList = AppData.Current.Codes.Where(x => x.Type == SelectedDetailStatus.Type).OrderBy(i => i.Order).ToList();
            }
        }

        AllDaysStr = FormatStringWithBold(Medication.DaysStr, ",", Medication.StartDateTime.ToString("ddd"));
        AllTimeSlotsStr = FormatStringWithBold(Medication.AllTimeSlot, ",", Medication.ApplicableSlot);

        IsEnquireOfficeVisible = false;
        EnableCompletionStatus = true;


        var now = DateTimeExtensions.NowNoTimezone();
        var medBoundTimeGracePeriodFrom = Medication.StartDateTime - new TimeSpan(default(int), Medication.GracePeriod, default(int));
        var medBoundTimeGracePeriodTo = Medication.EndDateTime + new TimeSpan(default(int), Medication.GracePeriod, default(int));
        var isInsideBounds = medBoundTimeGracePeriodFrom <= now && now <= medBoundTimeGracePeriodTo;
        var IsCompleted = Booking.IsCompleted.HasValue && Booking.IsCompleted.Value;


        IsMedicationActionMessageVisible = true;
        if (BaseVisitId == 0)
        {
            // No visit object, no need to show the input elements
            IsMedicationActionMessageVisible = false;
            EnableCompletionStatus = false;
            IsEnquireOfficeVisible = false;
            return;
        }

        if (IsCompleted) {
            // for completed booking, should show the input elements
            IsMedicationActionMessageVisible = false;
            ShowCompletionStatus = EnableCompletionStatus = true;
            IsEnquireOfficeVisible = false;
            return;
        }

        if (isInsideBounds)
        {
            MedicationActionMessage = EMedicationAdministerAction.Administer;
            IsMedicationActionMessageVisible = false;
            ShowCompletionStatus = EnableCompletionStatus = true;
            return;
        }

        ShowCompletionStatus = VisitMedication.CanAdmisterMedication.HasValue;

        if (VisitMedication.HasSentLateMedRequest)
        {
            // Already sent medication handshake
            MedicationActionMessage = !VisitMedication.CanAdmisterMedication.HasValue
                ? EMedicationAdministerAction.AwaitingConfirmation
                : (VisitMedication.HasNoResponse
                    ? EMedicationAdministerAction.NoResponse:
                    (VisitMedication.CanAdmisterMedication.Value
                        ? EMedicationAdministerAction.Proceed
                        : EMedicationAdministerAction.DoNotProceed));

            if (VisitMedication.CanAdmisterMedication.HasValue && !VisitMedication.CanAdmisterMedication.Value)
                EnableCompletionStatus = false;
        }
        else
        {
            // show Medication handshake trigger button
            IsEnquireOfficeVisible = true;
            MedicationActionMessage = EMedicationAdministerAction.SeekApproval;
        }
    }

    private async Task OnSubmit(bool isSuccess = false)
    {
        await SaveMedication();
    }

    private async Task OnReqMedicationAdministration()
    {
        VisitMedication.HasSentLateMedRequest = await AppServices.Current.MedicationService.RequestMedicationAdministration(Id);
        VisitMedication = await AppServices.Current.VisitMedicationService.InsertOrReplace(VisitMedication);

        await InitCommand.Execute(true);
    }

    private FormattedString FormatStringWithBold(string input, string delimiter, string matchValue = null)
    {
        var formattedString = new FormattedString();

        if (!string.IsNullOrEmpty(input))
        {
            var slots = input.Split(new[] { delimiter }, StringSplitOptions.TrimEntries);
            foreach (var slot in slots)
            {
                var trimmedSlot = slot.Trim();
                var span = new Span
                {
                    Text = trimmedSlot,
                    TextColor = Color.FromArgb("#1d1d1f")
                };

                if (!string.IsNullOrEmpty(matchValue) && string.Equals(trimmedSlot, matchValue, StringComparison.OrdinalIgnoreCase))
                {
                    span.FontAttributes = FontAttributes.Bold;
                    span.TextColor = Color.FromArgb("#000000");
                    span.FontSize = 15;
                }

                formattedString.Spans.Add(span);
                formattedString.Spans.Add(new Span { Text = ", " });
            }

            if (formattedString.Spans.Count > 0)
            {
                formattedString.Spans.RemoveAt(formattedString.Spans.Count - 1);
            }
        }

        return formattedString;
    }

    private new async Task OnBack()
    {
        if (VisitMedication != null && !VisitMedication.IsSaved && !IsReadOnly)
        {
            bool answer = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle, Messages.DialogUpdateConfirmation, Messages.Yes, Messages.No);
            if (answer)
                await SaveMedication(false);
        }

        await base.OnBack();
    }

    public async Task SaveMedication(bool fromSave = true)
    {
        if (VisitMedication.HasSentLateMedRequest && !VisitMedication.CanAdmisterMedication.HasValue)
        {
            await App.Current.MainPage.ShowSnackbar(Messages.MedicationSubmissionPendingApproval, false);
            return;
        }

        if (SelectedStatus == null)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.SelectStatusBeforeMedicationSubmit, false);
            return;
        }

        if (SelectedDetailStatus == null)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.SelectStatusDetailBeforeMedicationSubmit, false);
            return;
        }

        VisitMedication.IsSaved = VisitMedication.CompletedOn == null && fromSave;
        VisitMedication.CompletedOn = DateTimeExtensions.NowNoTimezone();
        VisitMedication.CompletionStatusId = SelectedStatus.Id;
        VisitMedication.CompletionStatusDetailId = SelectedDetailStatus.Id;
        VisitMedication = await AppServices.Current.VisitMedicationService.InsertOrReplace(VisitMedication);

        WeakReferenceMessenger.Default.Send(new MessagingEvents.BookingPageMedicationChangedMessage(BaseVisitId));

        if (fromSave) await Application.Current.MainPage.ShowSnackbar(Messages.DataStoreSuccess, true);

        await base.OnBack();
    }

    private async Task StatusChanged(Code code)
    {
        if (SelectedStatus != null)
        {
            var completionStatusList = new Dictionary<string, string>
            {
                { "COMPLETED", "MEDICATION_COMPLETED" },
                { "PARTIALLY_COMPLETED", "MEDICATION_PARTIAL_COMPLETED" },
                { "UNABLE_TO_COMPLETE", "MEDICATION_UNABLE_TO_COMPLETE" }
            };
            if (completionStatusList.TryGetValue(SelectedStatus.Name, out var statusType))
            {
                StatusDetailList = AppData.Current.Codes
                    .Where(x => x.Type.Equals(statusType, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(i => i.Order)
                    .ToList();
            }
        }
        await Task.CompletedTask;
    }

    public void SubscribeAllMessagingCenters()
    {
        var isBodyMapChangeRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.MedicationPageBodyMapChangedMessage>(this);
        if (!isBodyMapChangeRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.MedicationPageBodyMapChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (message.Value)
                    {
                        var bodyMaps = await AppServices.Current.BodyMapService.GetAllByVisitMedicationId(VisitMedication.Id);
                        BodyMapList = bodyMaps.OrderBy(i => i.AddedOn).ToList();
                    }
                });

        var medicateUpdateRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.MedicationPageUpdateReceivedMessage>(this);
        if (!medicateUpdateRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.MedicationPageUpdateReceivedMessage>(this,
                async (recipient, message) => await InitCommand.Execute(true));
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.MedicationPageBodyMapChangedMessage>(this);
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.MedicationPageUpdateReceivedMessage>(this);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var id))
            Id = Convert.ToInt32(id);
        if (query.TryGetValue(nameof(BaseVisitId), out var visitId))
            BaseVisitId = Convert.ToInt32(visitId);
        if (query.TryGetValue(nameof(IsReadOnly), out var isReadOnly))
            IsReadOnly = Convert.ToBoolean(isReadOnly);

        await InitCommand.Execute();
    }
}