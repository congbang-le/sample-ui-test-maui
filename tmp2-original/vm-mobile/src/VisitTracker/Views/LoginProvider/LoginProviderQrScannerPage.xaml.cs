namespace VisitTracker;

public partial class LoginProviderQrScannerPage : BaseContentPage<LoginProviderQrScannerVm>
{
    public LoginProviderQrScannerPage(LoginProviderQrScannerVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}