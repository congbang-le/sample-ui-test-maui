namespace VisitTracker;

public partial class ServiceUsersPage : BaseContentPage<ServiceUsersVm>
{
    public ServiceUsersPage(ServiceUsersVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}