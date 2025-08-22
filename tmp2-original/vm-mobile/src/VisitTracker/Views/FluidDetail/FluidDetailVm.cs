namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
[QueryProperty(nameof(BookingId), nameof(BookingId))]
[QueryProperty(nameof(BaseVisitId), nameof(BaseVisitId))]
[QueryProperty(nameof(IsReadOnly), nameof(IsReadOnly))]
public class FluidDetailVm : BaseVm, IQueryAttributable, IDisposable
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int BaseVisitId { get; set; }

    public VisitFluid VisitFluid { get; set; }
    public Booking Booking { get; set; }
    public ServiceUser ServiceUser { get; set; }
    public UserCardDto ServiceUserCard { get; set; }

    public IList<BodyMap> BodyMapList { get; set; }
    public List<Incident> IncidentList { get; set; }

    public string TypeName => EPageType.FluidType.ToString();

    public IList<AttachmentDto> AttachmentList { get; set; }

    public ReactiveCommand<bool, Unit> SubmitCommand { get; }
    public ReactiveCommand<Unit, Unit> OnBackCommand { get; }

    public FluidDetailVm()
    {
        SubmitCommand = ReactiveCommand.CreateFromTask<bool>(OnSubmit);
        OnBackCommand = ReactiveCommand.CreateFromTask(OnBack);

        BindBusyWithException(SubmitCommand);
        BindBusyWithException(OnBackCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        Booking = await AppServices.Current.BookingService.GetById(BookingId);
        ServiceUser = await AppServices.Current.ServiceUserService.GetById(Booking.ServiceUserId);
        ServiceUserCard = await BaseAssembler.BuildUserCardFromSu(ServiceUser, Booking);

        if (Id <= 0)
            VisitFluid = new VisitFluid
            {
                VisitId = BaseVisitId,
                ServiceUserId = Booking.ServiceUserId
            };
        else VisitFluid = await AppServices.Current.VisitFluidService.GetById(Id);

        BodyMapList = (await AppServices.Current.BodyMapService.GetAllByFluidId(VisitFluid.Id)).ToList();
        var attachments = await AppServices.Current.AttachmentService.GetAllByFluidId(VisitFluid.Id);
        AttachmentList = BaseAssembler.BuildAttachmentViewModels(attachments);
    }

    private async Task OnSubmit(bool isSuccess = false)
    {
        await SaveFluid();
    }

    private new async Task OnBack()
    {
        if (VisitFluid != null && !VisitFluid.IsSaved && !IsReadOnly)
        {
            bool answer = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle, Messages.DialogUpdateConfirmation, Messages.Yes, Messages.No);
            if (answer)
                await SaveFluid(false);
        }

        await base.OnBack();
    }

    public async Task SaveFluid(bool fromSave = true)
    {
        if (new[] { VisitFluid.IvScIntake, VisitFluid.OralIntake, VisitFluid.OtherIntake }
            .ToList().All(i => i == null || i == 0))
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.FluidAnyOneInputRequired, false);
            return;
        }

        if (new[] { VisitFluid.OtherOutput, VisitFluid.TubeOutput, VisitFluid.UrineOutput, VisitFluid.VomitOutput }
            .ToList().All(i => i == null || i == 0))
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.FluidAnyOneOutputRequired, false);
            return;
        }

        VisitFluid.IsSaved = VisitFluid.CompletedOn == null && fromSave;
        VisitFluid.CompletedOn = DateTimeExtensions.NowNoTimezone();
        VisitFluid = await AppServices.Current.VisitFluidService.InsertOrReplace(VisitFluid);

        WeakReferenceMessenger.Default.Send(new MessagingEvents.BookingPageFluidChangedMessage(BaseVisitId));

        if (fromSave) await Application.Current.MainPage.ShowSnackbar(Messages.DataStoreSuccess, true);
        await base.OnBack();
    }

    public void SubscribeAllMessagingCenters()
    {
        var isBodyMapChangeRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.FluidPageBodyMapChangedMessage>(this);
        if (!isBodyMapChangeRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.FluidPageBodyMapChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (message.Value)
                    {
                        var bodyMaps = await AppServices.Current.BodyMapService.GetAllByFluidId(VisitFluid.Id);
                        BodyMapList = bodyMaps.OrderBy(i => i.AddedOn).ToList();
                    }
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.FluidPageBodyMapChangedMessage>(this);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var id))
            Id = Convert.ToInt32(id);
        if (query.TryGetValue(nameof(BaseVisitId), out var baseVisitId))
            BaseVisitId = Convert.ToInt32(baseVisitId);
        if (query.TryGetValue(nameof(BookingId), out var bookingId))
            BookingId = Convert.ToInt32(bookingId);
        if (query.TryGetValue(nameof(IsReadOnly), out var isReadOnly))
            IsReadOnly = Convert.ToBoolean(isReadOnly);

        await InitCommand.Execute();
    }
}