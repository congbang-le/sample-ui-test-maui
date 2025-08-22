namespace VisitTracker;

public partial class LoginPage : BaseContentPage<LoginVm>
{
    public LoginPage(LoginVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            layout.TranslateTo(0, -200, 50);
        }
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            layout.TranslateTo(0, 0, 50);
        }
    }
}