
namespace VisitTracker;

[QueryProperty(nameof(Domain.Incident), nameof(Incident))]
[QueryProperty(nameof(OnlineIncidentId), nameof(OnlineIncidentId))]
[QueryProperty(nameof(BaseVisitId), nameof(BaseVisitId))]
[QueryProperty(nameof(BookingId), nameof(BookingId))]
[QueryProperty(nameof(ServiceUserId), nameof(ServiceUserId))]
[QueryProperty(nameof(IsReadOnly), nameof(IsReadOnly))]
public class IncidentReportVm : BaseVm, IQueryAttributable, IDisposable
{
    public int OnlineIncidentId { get; set; }
    public int BaseVisitId { get; set; }
    public int BookingId { get; set; }
    public int ServiceUserId { get; set; }

    public Incident Incident { get; set; }
    public ServiceUser ServiceUser { get; set; }
    public Booking Booking { get; set; }
    public ServiceUserAddress ServiceUserAddress { get; set; }
    public UserCardDto ServiceUserCard { get; set; }
    public IList<BodyMap> BodyMapList { get; set; }
    public ReactiveCommand<Unit, Unit> OpenDeleteConfirmationCommand { get; set; }

    [Reactive] public DateTime IncidentDate { get; set; }
    [Reactive] public TimeSpan IncidentTime { get; set; }
    [Reactive] public Code SelectedInjury { get; set; }
    [Reactive] public Code SelectedTreatment { get; set; }
    [Reactive] public Code SelectedType { get; set; }

    public IList<AttachmentDto> AttachmentList { get; set; }
    public string TypeName => EPageType.IncidentType.ToString();

    public List<Code> TypeList { get; set; }
    public List<Code> InjuryList { get; set; }
    public List<Code> TreatmentList { get; set; }
    public bool IsFromOngoing { get; set; } = true;
    public bool IsTypeOther { get; set; }
    public bool IsInjuryOther { get; set; }
    public bool AtSuLocation { get; set; }
    public ReactiveCommand<Unit, Unit> ChangeLocationCommand { get; }
    public ReactiveCommand<Code, Unit> OtherChangeCommand { get; }
    public ReactiveCommand<Unit, Unit> OnBackCommand { get; }
    public ReactiveCommand<bool, Unit> SubmitCommand { get; }
    private bool _isLoaded = false;
    public ImageSource ToolbarIconSource{ get;set; }
    public IList<Attachment> Attachments { get; set; }

    public IncidentReportVm()
    {
        ChangeLocationCommand = ReactiveCommand.CreateFromTask(ChangeLocation);
        OtherChangeCommand = ReactiveCommand.Create<Code>(OtherTypeChange);
        SubmitCommand = ReactiveCommand.CreateFromTask<bool>(OnSubmit);
        OnBackCommand = ReactiveCommand.CreateFromTask(OnBack);
        OpenDeleteConfirmationCommand = ReactiveCommand.CreateFromTask(OpenDeleteConfirmation);

        BindBusyWithException(ChangeLocationCommand, true);
        BindBusyWithException(OtherChangeCommand);
        BindBusyWithException(OnBackCommand);
        BindBusyWithException(SubmitCommand);
        BindBusyWithException(OpenDeleteConfirmationCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        if (Incident == null)
        {
            Incident = new Incident();
            IncidentDate = DateTimeExtensions.NowNoTimezone().Date;
            IncidentTime = DateTimeExtensions.NowNoTimezone().TimeOfDay;
            Incident.UnSaved = true;
            Incident.VisitId = BaseVisitId > 0 ? BaseVisitId : null;
            Incident.AtSuLocation = BaseVisitId > 0 ? true : false;
            Incident.DuringOnGoingVisit = BaseVisitId > 0 ? true : false;
            ToolbarIconSource = new FontImageSource
                {
                FontFamily = "MaterialCommunityIcons",
                    Glyph = "\uF1C0",
                    Size = 24,
                    Color = Colors.White
                };
        }
        else
        {
            IncidentDate = Incident.IncidentDateTime.Date;
            IncidentTime = Incident.IncidentDateTime.TimeOfDay;
            ToolbarIconSource = new FontImageSource
            {
                FontFamily = "MaterialCommunityIcons",
                Glyph = "\uF1C0",
                Size = 24,
                Color = Colors.White.WithAlpha(0.6f)
            };
        }
        
       
        if (OnlineIncidentId != default)
        {
            var incidentDetail = await AppServices.Current.IncidentService.GetIncidentDetail(OnlineIncidentId);
            Incident = incidentDetail.Incident;
            BodyMapList = incidentDetail.BodyMaps;
            ServiceUserId = incidentDetail.ServiceUserId;
            incidentDetail.Attachments.ToList().ForEach(i => i.FileName = Path.GetFileName(i.S3Url));

            Incident = await AppServices.Current.IncidentService.InsertOrReplace(Incident);
            BodyMapList = await AppServices.Current.BodyMapService.InsertOrReplace(BodyMapList);
            incidentDetail.Attachments = await AppServices.Current.AttachmentService.InsertOrReplace(incidentDetail.Attachments);

            Attachments = incidentDetail.Attachments.Where(x => x.IncidentId != null).ToList();
        }
        else
        {
            BodyMapList = await AppServices.Current.BodyMapService.GetAllByIncidentId(Incident.Id);
            Attachments = await AppServices.Current.AttachmentService.GetAllByIncidentId(Incident.Id);
        }

        ServiceUser = await AppServices.Current.ServiceUserService.GetById(ServiceUserId);
        ServiceUserAddress = await AppServices.Current.ServiceUserAddressService.GetActiveAddressNow(ServiceUserId);

        AttachmentList = BaseAssembler.BuildAttachmentViewModels(Attachments);

        if (Booking != null)
        {
            if (BookingId != default) Booking = await AppServices.Current.BookingService.GetById(BookingId);
            else if (BaseVisitId > 0)
            {
                var visit = await AppServices.Current.VisitService.GetById(BaseVisitId);
                var bookingDetail = await AppServices.Current.BookingDetailService.GetById(visit.BookingDetailId);
                Booking = await AppServices.Current.BookingService.GetById(bookingDetail.BookingId);
            }
        }
        ServiceUserCard = await BaseAssembler.BuildUserCardFromSu(ServiceUser, Booking);

        TypeList = AppData.Current.Codes?.Where(x => x.Type == ECodeType.INCIDENT_TYPE.ToString()).OrderBy(i => i.Order).ToList();
        InjuryList = AppData.Current.Codes?.Where(x => x.Type == ECodeType.INJURY.ToString()).OrderBy(i => i.Order).ToList();
        TreatmentList = AppData.Current.Codes?.Where(x => x.Type == ECodeType.TREATMENT.ToString()).OrderBy(i => i.Order).ToList();
        if (Incident.InjuryId > 0)
            SelectedInjury = InjuryList.FirstOrDefault(i => i.Id == Incident.InjuryId);
        if (Incident.TreatmentId > 0)
            SelectedTreatment = TreatmentList.FirstOrDefault(i => i.Id == Incident.TreatmentId);
        if (Incident.TypeId > 0)
            SelectedType = TypeList.FirstOrDefault(i => i.Id == Incident.TypeId);

        AtSuLocation = Incident.AtSuLocation;

        if (Incident.Location == null)
        {
            Incident.Location = AtSuLocation ? ServiceUserAddress.Address : string.Empty;
        }

        _isLoaded = true;
    }

    private async Task ChangeLocation()
    {
        if (!_isLoaded)
            return; 

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            Incident.AtSuLocation = AtSuLocation;
            Incident.Location = Incident.AtSuLocation ? ServiceUserAddress.Address : string.Empty;
        });
    }

    protected async Task OpenDeleteConfirmation()
    {
        if (Incident == null)
            return;

        bool answer = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle, Messages.DialogDeleteConfirmation, Messages.Yes, Messages.No);
        if (answer)
        {
            await AppServices.Current.AttachmentService.DeleteAllByIncidentId(Incident.Id);
            var bodyMaps = await AppServices.Current.BodyMapService.GetAllByIncidentId(Incident.Id);
            if (bodyMaps != null && bodyMaps.Any())
            {
                foreach (var bodyMap in bodyMaps)
                    await AppServices.Current.AttachmentService.DeleteAllByBodyMapId(bodyMap.Id);

                await AppServices.Current.BodyMapService.DeleteAllByIncidentId(Incident.Id);
            }
            await AppServices.Current.IncidentService.DeleteAllById(Incident.Id);
            WeakReferenceMessenger.Default.Send(new MessagingEvents.BookingPageIncidentReportChangedMessage(BaseVisitId));
            await Application.Current.MainPage.ShowSnackbar(Messages.DeleteSuccess, true);
        }
        await base.OnBack();
    }

    private void OtherTypeChange(Code selection)
    {
        IsTypeOther = SelectedType?.Name == ECodeType.OTHER.ToString();
        IsInjuryOther = SelectedInjury?.Name == ECodeType.OTHER.ToString();

        Incident.OtherIncidentType = SelectedType?.Name == ECodeType.OTHER.ToString()
            ? Incident.OtherIncidentType : null;
        Incident.OtherInjury = SelectedInjury?.Name == ECodeType.OTHER.ToString()
            ? Incident.OtherInjury : null;
    }

    private async Task OnSubmit(bool isSuccess = false) => await SaveIncidentReport();

    private new async Task OnBack()
    {
        if (Incident != null && !IsReadOnly)
        {
            var message = Incident.UnSaved ? Messages.DialogSaveConfirmation : Messages.DialogUpdateConfirmation;
            bool answer = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle,
                            message, Messages.Yes, Messages.No);
            if (answer)
            {
                await SaveIncidentReport(false);
                return;
            }
            else
            {
                var allAttachments = await AppServices.Current.AttachmentService.GetAllByIncidentId(Incident.Id);
                if (allAttachments != null && allAttachments.Any())
                {
                    var newAttachments = Attachments == null || !Attachments.Any() ? allAttachments
                        : allAttachments.Where(x => !Attachments.Select(y => y.Id).ToList().Contains(x.Id)).ToList();
                    if (newAttachments != null && newAttachments.Any())
                        await AppServices.Current.AttachmentService.DeleteAllByIds(newAttachments.Select(y => y.Id).ToList());
                }
            }
        }

        await base.OnBack();
    }

    private async Task SaveIncidentReport(bool fromSave = true)
    {
        if (SelectedInjury == null)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.IncidentInjurySelectionRequired, false);
            return;
        }

        if (SelectedType == null)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.IncidentTypeSelectionRequired, false);
            return;
        }

        if (SelectedTreatment == null)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.IncidentTreatmentSelectionRequired, false);
            return;
        }

        if (string.IsNullOrEmpty(Incident.Location?.Trim()))
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.IncidentLocationRequired, false);
            return;
        }

        if (SelectedType?.Name == ECodeType.OTHER.ToString() && string.IsNullOrEmpty(Incident.OtherIncidentType))
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.IncidentOtherTypeRequired, false);
            return;
        }

        if (SelectedInjury?.Name == ECodeType.OTHER.ToString() && string.IsNullOrEmpty(Incident.OtherInjury))
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.IncidentOtherInjuryRequired, false);
            return;
        }

        if (string.IsNullOrEmpty(Incident.Summary))
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.IncidentSummaryRequired, false);
            return;
        }

        var now = DateTimeExtensions.NowNoTimezone();

        Incident.IsSaved = Incident.CompletedOn == null && fromSave;
        Incident.UnSaved = false;

        if (fromSave)
        {
            Incident.SubmittedOn = now;
            Incident.SubmittedById = AppData.Current.CurrentProfile.Id;
            Incident.SubmittedByType = AppData.Current.CurrentProfile.Type;
            Incident.SubmittedByName = AppData.Current.CurrentProfile.Name;

            Incident.InjuryId = SelectedInjury.Id;
            Incident.TypeId = SelectedType.Id;
            Incident.TreatmentId = SelectedTreatment.Id;
            Incident.ServiceUserId = ServiceUser.Id;

            var geolocation = await GeolocationExtensions.GetBestLocationAsync();
            Incident.Geolocation = geolocation.ToLatLonString();

            Incident.VisitId = BaseVisitId > 0 ? BaseVisitId : null;
            Incident.CompletedOn = now;
            Incident.IncidentDateTime = IncidentDate.Add(IncidentTime);

            if (BaseVisitId > 0)
                Incident.DisplayName = $"Incident {now}";

            if (BaseVisitId > 0) Incident = await AppServices.Current.IncidentService.InsertOrReplace(Incident);
            else
            {
                var attachments = await AppServices.Current.AttachmentService.GetAllByIncidentId(Incident.Id);
                var bodyMaps = await AppServices.Current.BodyMapService.GetAllByIncidentId(Incident.Id);
                var bodyMapAttachments = await AppServices.Current.AttachmentService.GetAllByIds(
                    bodyMaps.Select(x => x.Id), "BodyMapId");

                List<Attachment> allAttachments = new List<Attachment>();
                if (attachments != null && attachments.Any())
                    allAttachments.AddRange(attachments);
                if (bodyMapAttachments != null && bodyMapAttachments.Any())
                    allAttachments.AddRange(bodyMapAttachments);

                if (allAttachments != null && allAttachments.Any())
                    await AttachmentHelper.Current.UploadFiles(allAttachments,
                         string.Join('-', new[] { now.ToString("yyyy"), now.ToString("MM") }), "Incident");

                var incidentAdhoc = new IncidentAdhocRequest
                {
                    Incident = Incident,
                    Attachments = allAttachments?.ToList(),
                    BodyMaps = bodyMaps?.ToList(),
                };
                var isSuccess = await AppServices.Current.IncidentService.UploadIncidentReportAdhoc(incidentAdhoc);
                if (isSuccess)
                {
                    await AppServices.Current.AttachmentService.DeleteAllByIds(allAttachments.Select(x => x.Id));
                    await AppServices.Current.BodyMapService.DeleteAllByIds(bodyMaps.Select(x => x.Id));
                    await AppServices.Current.IncidentService.DeleteAllById(Incident.Id);
                }
                else
                {
                    await Application.Current.MainPage.ShowSnackbar(Messages.IncidentAdhocUploadFailed, false);
                    return;
                }
            }

            await Application.Current.MainPage.ShowSnackbar(Messages.DataStoreSuccess, true);
            WeakReferenceMessenger.Default.Send(new MessagingEvents.BookingPageIncidentReportChangedMessage(BaseVisitId));
        }

        await base.OnBack();
    }

    public void SubscribeAllMessagingCenters()
    {
        var isBodyMapChangeRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.IncidentPageBodyMapChangedMessage>(this);
        if (!isBodyMapChangeRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.IncidentPageBodyMapChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (message.Value)
                    {
                        var bodyMaps = await AppServices.Current.BodyMapService.GetAllByIncidentId(Incident.Id);
                        BodyMapList = bodyMaps.OrderBy(i => i.AddedOn).ToList();
                    }
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.IncidentPageBodyMapChangedMessage>(this);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(OnlineIncidentId), out var onlineIncidentId))
            OnlineIncidentId = Convert.ToInt32(onlineIncidentId);
        if (query.TryGetValue(nameof(BaseVisitId), out var visitId))
            BaseVisitId = Convert.ToInt32(visitId);
        if (query.TryGetValue(nameof(BookingId), out var bookingId))
            BookingId = Convert.ToInt32(bookingId);
        if (query.TryGetValue(nameof(ServiceUserId), out var serviceUserId))
            ServiceUserId = Convert.ToInt32(serviceUserId);
        if (query.TryGetValue(nameof(Domain.Incident), out var incident))
            Incident = incident as Incident;
        if (query.TryGetValue(nameof(IsReadOnly), out var isReadOnly))
            IsReadOnly = Convert.ToBoolean(isReadOnly);

        await InitCommand.Execute();
    }
}