namespace VisitTracker;

public partial class CareWorkerDashboardPage : BaseContentPage<CareWorkerDashboardVm>
{
    public CareWorkerDashboardPage(CareWorkerDashboardVm viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}