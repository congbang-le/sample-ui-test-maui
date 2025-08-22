namespace VisitTracker;

public partial class SupervisorHomePage : ReactiveShell<SupervisorHomeVm>
{
    public SupervisorHomePage(SupervisorHomeVm viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}