namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
[QueryProperty(nameof(BaseVisitId), nameof(BaseVisitId))]
[QueryProperty(nameof(IsReadOnly), nameof(IsReadOnly))]
public class TaskDetailVm : BaseVm, IQueryAttributable, IDisposable
{
    public int Id { get; set; }
    public int BaseVisitId { get; set; }

    public BookingTask Task { get; set; }
    public VisitTask VisitTask { get; set; }
    public Booking Booking { get; set; }
    public ServiceUser ServiceUser { get; set; }
    public UserCardDto ServiceUserCard { get; set; }

    public IList<Code> StatusList { get; set; }
    [Reactive] public Code SelectedStatus { get; set; }

    public string TypeName => EPageType.TaskType.ToString();

    public IList<AttachmentDto> AttachmentList { get; set; }
    public IList<BodyMap> BodyMapList { get; set; }

    public ReactiveCommand<bool, Unit> SubmitCommand { get; }
    public ReactiveCommand<Unit, Unit> OnBackCommand { get; }

    public TaskDetailVm()
    {
        SubmitCommand = ReactiveCommand.CreateFromTask<bool>(OnSubmit);
        OnBackCommand = ReactiveCommand.CreateFromTask(OnBack);

        BindBusyWithException(SubmitCommand);
        BindBusyWithException(OnBackCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        if (BaseVisitId == default)
        {
            await Shell.Current.Navigation.PopAsync();
            await Application.Current.MainPage.ShowSnackbar(Messages.ExceptionOccurred, false);
            return;
        }

        Task = await AppServices.Current.TaskService.GetById(Id);
        VisitTask = await AppServices.Current.VisitTaskService.GetByTaskAndVisitId(Task.Id, BaseVisitId);
        if (VisitTask == null)
        {
            VisitTask = new VisitTask();
            VisitTask.TaskId = Task.Id;
            VisitTask.VisitId = BaseVisitId;
            VisitTask = await AppServices.Current.VisitTaskService.InsertOrReplace(VisitTask);
        }

        BodyMapList = (await AppServices.Current.BodyMapService.GetAllByVisitTaskId(VisitTask.Id)).OrderBy(i => i.AddedOn).ToList();
        var attachments = await AppServices.Current.AttachmentService.GetAllByVisitTaskId(VisitTask.Id);
        AttachmentList = BaseAssembler.BuildAttachmentViewModels(attachments);

        Booking = await AppServices.Current.BookingService.GetById(Task.BookingId);
        ServiceUser = await AppServices.Current.ServiceUserService.GetById(Booking.ServiceUserId);
        ServiceUserCard = await BaseAssembler.BuildUserCardFromSu(ServiceUser, Booking);

        StatusList = AppData.Current.Codes.Where(i => i.Type == ECodeType.TASK_COMPLETION.ToString()).OrderBy(i => i.Order)
            .Select(i => new Code
            {
                Name = char.ToUpper(i.Name[0]) + i.Name.Substring(1).ToLower(),
                DisplayName = i.DisplayName,
                Description = i.Description,
                Order = i.Order,
                Color = i.Color,
                Type = i.Type,
                Id = i.Id
            })
            .ToList();
        SelectedStatus = StatusList.FirstOrDefault(I => I.Id == VisitTask.CompletionStatusId);
    }

    private async Task OnSubmit(bool isSuccess = false)
    {
        await SaveTask();
    }

    private new async Task OnBack()
    {
        if (VisitTask != null && VisitTask.IsSaved && !IsReadOnly)
        {
            bool answer = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle, Messages.DialogUpdateConfirmation, Messages.Yes, Messages.No);
            if (answer)
                await SaveTask(false);
        }

        await base.OnBack();
    }

    public async Task SaveTask(bool fromSave = true)
    {
        if (SelectedStatus == null)
        {
            await Application.Current.MainPage.ShowSnackbar(Messages.SelectStatusBeforeTaskSubmit, false);
            return;
        }

        VisitTask.IsSaved = VisitTask.CompletedOn == null && fromSave;
        VisitTask.CompletedOn = DateTimeExtensions.NowNoTimezone();
        VisitTask.CompletionStatusId = SelectedStatus.Id;
        VisitTask = await AppServices.Current.VisitTaskService.InsertOrReplace(VisitTask);

        WeakReferenceMessenger.Default.Send(new MessagingEvents.BookingPageTaskChangedMessage(BaseVisitId));

        if (fromSave) await Application.Current.MainPage.ShowSnackbar(Messages.DataStoreSuccess, true);
        await base.OnBack();
    }

    public void SubscribeAllMessagingCenters()
    {
        var isBodyMapChangeRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.TaskPageBodyMapChangedMessage>(this);
        if (!isBodyMapChangeRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.TaskPageBodyMapChangedMessage>(this,
                async (recipient, message) =>
                {
                    if (message.Value)
                    {
                        var bodyMaps = await AppServices.Current.BodyMapService.GetAllByVisitTaskId(VisitTask.Id);
                        BodyMapList = bodyMaps.OrderBy(i => i.AddedOn).ToList();
                    }
                });
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.TaskPageBodyMapChangedMessage>(this);
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