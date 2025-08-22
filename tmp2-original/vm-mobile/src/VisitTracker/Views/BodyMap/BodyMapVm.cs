namespace VisitTracker;

[QueryProperty(nameof(Domain.BodyMap), nameof(BodyMap))]
[QueryProperty(nameof(BaseVisitId), nameof(BaseVisitId))]
[QueryProperty(nameof(TypeId), nameof(TypeId))]
[QueryProperty(nameof(Type), nameof(Type))]
[QueryProperty(nameof(IsReadOnly), nameof(IsReadOnly))]
public class BodyMapVm : BaseVm, IQueryAttributable
{
    public int Id { get; set; }
    public int BaseVisitId { get; set; }

    public int TypeId { get; set; }
    public string Type { get; set; }

    public IList<AttachmentDto> AttachmentList { get; set; }
    public BodyMap BodyMap { get; set; }
    [Reactive] public BodyMapCreateDto BodyMapCreate { get; set; }

    public BodyMapPage Page { get; set; }
    public BodyMapNotesPopupVm BodyMapNotesPopupVm { get; set; }

    public ReactiveCommand<Unit, Unit> OpenBodyMapRemarksPopupCommand { get; }
    public ReactiveCommand<Unit, Unit> ChangeBodyMapCommand { get; }
    public ReactiveCommand<Unit, Unit> OnSubmitCommand { get; }
    public ReactiveCommand<Unit, Unit> OnBackCommand { get; }

    public IList<Attachment> ExistingAttachments { get; set; }

    public BodyMapVm()
    {
        OpenBodyMapRemarksPopupCommand = ReactiveCommand.CreateFromTask(OpenBodyMapRemarksPopup);
        ChangeBodyMapCommand = ReactiveCommand.Create(ChangeBodyMap);
        OnSubmitCommand = ReactiveCommand.CreateFromTask(OnSubmit);
        OnBackCommand = ReactiveCommand.CreateFromTask(OnBack);

        BindBusyWithException(OpenBodyMapRemarksPopupCommand);
        BindBusyWithException(ChangeBodyMapCommand);
        BindBusyWithException(OnSubmitCommand);
        BindBusyWithException(OnBackCommand);
    }

    protected override async Task Init()
    {
        if (Type != EPageType.IncidentType.ToString() && BaseVisitId == default)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.ExceptionOccurred, false);
            await Shell.Current.Navigation.PopAsync();
            return;
        }

        if (BodyMap == null)
        {
            BodyMap = new BodyMap
            {
                BaseVisitId = BaseVisitId,
                UnSaved = true
            };
            BodyMap = await AppServices.Current.BodyMapService.InsertOrReplace(BodyMap);
        }

        if (Type == EPageType.BookingType.ToString()) BodyMap.VisitId = TypeId;
        else if (Type == EPageType.TaskType.ToString()) BodyMap.VisitTaskId = TypeId;
        else if (Type == EPageType.MedicationType.ToString()) BodyMap.VisitMedicationId = TypeId;
        else if (Type == EPageType.FluidType.ToString()) BodyMap.VisitFluidId = TypeId;
        else if (Type == EPageType.IncidentType.ToString()) BodyMap.IncidentId = TypeId;

        ExistingAttachments = await AppServices.Current.AttachmentService.GetAllByBodyMapId(
            BodyMap.ServerRef != default ? BodyMap.ServerRef : BodyMap.Id);
        var bodyMapVm = BaseAssembler.BuildBodyMapPopupViewModels(BodyMap, ExistingAttachments);
        BodyMapCreate = bodyMapVm.Item1;
        BodyMapNotesPopupVm = bodyMapVm.Item2;
    }

    private async Task OpenBodyMapRemarksPopup()
    {
        if (BodyMapNotesPopupVm == null || !(string.IsNullOrEmpty(BodyMapNotesPopupVm.Parts)))
        {
            var remarksPopup = new BodyMapNotesPopup(BodyMapNotesPopupVm == null ? new BodyMapNotesPopupVm()
            {
                Id = BodyMap.Id,
                BaseVisitId = BaseVisitId,
                IsReadOnly = IsReadOnly,
            } : BodyMapNotesPopupVm);

            BodyMapNotesPopupVm.IsReadOnly = IsReadOnly;
            BodyMapNotesPopupVm = (BodyMapNotesPopupVm)await PopupExtensions.ShowPopupAsync(Page, remarksPopup);
            if (BodyMapNotesPopupVm.IsAllowToSave) await OnSubmit();
        }
        else await App.Current.MainPage.DisplayAlert(Messages.Error, Messages.BodyMapNoSelection, Messages.Ok);
    }

    public void ChangeBodyMap()
    {
        BodyMapCreate.BodyMapLabel = BodyMapCreate.BodyMapLabel == Constants.BodyMapBack ?
            Constants.BodyMapFront : Constants.BodyMapBack;
    }

    private async Task OnSubmit()
    {
        await SaveBodyMap();
        await base.OnBack();
    }

    private new async Task OnBack()
    {
        if (BodyMap != null && !IsReadOnly)
        {
            var message = (BodyMap.UnSaved) ? Messages.DialogSaveConfirmation : Messages.DialogUpdateConfirmation;
            bool answer = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle,
                            message, Messages.Yes, Messages.No);
            if (answer) await SaveBodyMap();
            else
            {
                var allAttachments = await AppServices.Current.AttachmentService.GetAllByBodyMapId(
                    BodyMap.ServerRef != default ? BodyMap.ServerRef : BodyMap.Id);
                if (allAttachments != null && allAttachments.Any())
                {
                    var newAttachments = ExistingAttachments == null || !ExistingAttachments.Any() ? allAttachments
                        : allAttachments.Where(x => !ExistingAttachments.Select(y => y.Id).ToList().Contains(x.Id)).ToList();
                    if (newAttachments != null && newAttachments.Any())
                        await AppServices.Current.AttachmentService.DeleteAllByIds(newAttachments.Select(y => y.Id).ToList());
                }

                if (BodyMap.UnSaved) await AppServices.Current.BodyMapService.DeleteAllById(BodyMap.Id);
            }
        }

        await base.OnBack();
    }

    private async Task SaveBodyMap()
    {
        BodyMap.BaseVisitId = BaseVisitId;
        BodyMap.Parts = BodyMapNotesPopupVm?.Parts;
        BodyMap.Notes = BodyMapNotesPopupVm?.Summary;
        BodyMap.UnSaved = false;

        BodyMap.AddedOn = DateTimeExtensions.NowNoTimezone();
        BodyMap = await AppServices.Current.BodyMapService.InsertOrReplace(BodyMap);

        if (Type == EPageType.BookingType.ToString())
            WeakReferenceMessenger.Default.Send(new MessagingEvents.BookingPageBodyMapChangedMessage(BaseVisitId));
        else if (Type == EPageType.TaskType.ToString())
            WeakReferenceMessenger.Default.Send(new MessagingEvents.TaskPageBodyMapChangedMessage(true));
        else if (Type == EPageType.MedicationType.ToString())
            WeakReferenceMessenger.Default.Send(new MessagingEvents.MedicationPageBodyMapChangedMessage(true));
        else if (Type == EPageType.FluidType.ToString())
            WeakReferenceMessenger.Default.Send(new MessagingEvents.FluidPageBodyMapChangedMessage(true));
        else if (Type == EPageType.IncidentType.ToString())
            WeakReferenceMessenger.Default.Send(new MessagingEvents.IncidentPageBodyMapChangedMessage(true));

        await Application.Current.MainPage.ShowSnackbar(Messages.DataStoreSuccess, true);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Domain.BodyMap), out var bodyMap))
            BodyMap = bodyMap as BodyMap;
        if (query.TryGetValue(nameof(BaseVisitId), out var baseVisitId))
            BaseVisitId = Convert.ToInt32(baseVisitId);
        if (query.TryGetValue(nameof(TypeId), out var typeId))
            TypeId = Convert.ToInt32(typeId);
        if (query.TryGetValue(nameof(Type), out var type))
            Type = type as string;
        if (query.TryGetValue(nameof(IsReadOnly), out var isReadOnly))
            IsReadOnly = Convert.ToBoolean(isReadOnly);

        await InitCommand.Execute();
    }
}