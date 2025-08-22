namespace VisitTracker;

public class ErrorVm : BaseVm, IDisposable
{
    public List<ErrorDto> Errors { get; set; }

    public ReactiveCommand<Unit, Unit> RetryCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

    public ErrorVm()
    {
        RetryCommand = ReactiveCommand.CreateFromTask(Retry);
        OpenSettingsCommand = ReactiveCommand.Create(OpenSettings);

        BindBusyWithException(RetryCommand);
        BindBusyWithException(OpenSettingsCommand);
    }

    protected override async Task Init()
    {
        SubscribeAllMessagingCenters();

        Errors = await App.FeaturePermissionHandlerService.CheckEverything();
        if (Errors == null || !Errors.Any())
            await Shell.Current.Navigation.PopAsync();
    }

    private async Task Retry() => await InitCommand.Execute(true);

    private void OpenSettings() => App.FeaturePermissionHandlerService.OpenSettingsPage();

    public void SubscribeAllMessagingCenters()
    {
        var permissionsUpdatedRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.PermissionsUpdatedMessage>(this);
        if (!permissionsUpdatedRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.PermissionsUpdatedMessage>(this,
                async (recipient, message) => await InitCommand.Execute(true));
    }

    public void Dispose() => UnsubscribeAllMessagingCenters();

    public void UnsubscribeAllMessagingCenters()
    {
        WeakReferenceMessenger.Default.Unregister<MessagingEvents.PermissionsUpdatedMessage>(this);
    }
}