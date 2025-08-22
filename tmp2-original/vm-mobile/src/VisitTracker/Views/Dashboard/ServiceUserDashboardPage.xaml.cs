namespace VisitTracker;

public partial class ServiceUserDashboardPage : BaseContentPage<ServiceUserDashboardVm>
{
    public ServiceUserDashboardPage(ServiceUserDashboardVm viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}