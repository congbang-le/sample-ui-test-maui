namespace VisitTracker;

public partial class SupervisorDashboardPage : BaseContentPage<SupervisorDashboardVm>
{
    public SupervisorDashboardPage(SupervisorDashboardVm viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}