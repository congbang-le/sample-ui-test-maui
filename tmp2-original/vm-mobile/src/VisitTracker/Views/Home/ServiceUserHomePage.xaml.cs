namespace VisitTracker;

public partial class ServiceUserHomePage : ReactiveShell<ServiceUserHomeVm>
{
    public ServiceUserHomePage(ServiceUserHomeVm viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}