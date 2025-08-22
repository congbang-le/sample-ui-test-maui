namespace VisitTracker;

public partial class LoginProviderPage : BaseContentPage<LoginProviderVm>
{
    public LoginProviderPage(LoginProviderVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}