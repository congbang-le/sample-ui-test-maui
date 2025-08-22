namespace VisitTracker;

public partial class EmergencyButtonControl : BaseContentView
{
    public ReactiveCommand<Unit, Unit> OpenEmergencyButtonCommand { get; set; }

    public EmergencyButtonControl()
    {
        InitializeComponent();

        OpenEmergencyButtonCommand = ReactiveCommand.CreateFromTask(OpenEmergency);
        BindBusy(OpenEmergencyButtonCommand);
    }

    public async Task OpenEmergency()
    {
        string action = await App.Current.MainPage.DisplayActionSheet(Messages.Emergency, Messages.Cancel, null, Messages.Ambulance, Messages.Fire);
        switch (action)
        {
            case "Ambulance":
                await SystemHelper.Current.Open("999");
                break;

            case "Fire":
                await SystemHelper.Current.Open("999");
                break;
        }
    }
}