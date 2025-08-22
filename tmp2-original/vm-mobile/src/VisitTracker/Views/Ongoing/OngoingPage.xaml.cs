namespace VisitTracker;

public partial class OngoingPage : BaseContentPage<OngoingVm>
{
    public OngoingPage(OngoingVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}