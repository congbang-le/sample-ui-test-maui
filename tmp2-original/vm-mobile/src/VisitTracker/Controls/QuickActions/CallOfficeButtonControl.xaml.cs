namespace VisitTracker;

public partial class CallOfficeButtonControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenCallOfficeCommand { get; set; }

    public CallOfficeButtonControl()
    {
        InitializeComponent();

        OpenCallOfficeCommand = ReactiveCommand.CreateFromTask(Call);
        BindBusy(OpenCallOfficeCommand);
    }

    private async Task Call()
    {
        var provider = await AppServices.Current.ProviderService.GetLoggedInProvider();
        await SystemHelper.Current.Open(provider.Phone);
    }
}