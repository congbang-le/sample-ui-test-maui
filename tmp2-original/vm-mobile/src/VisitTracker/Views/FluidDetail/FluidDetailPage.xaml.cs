namespace VisitTracker;

public partial class FluidDetailPage : BaseContentPage<FluidDetailVm>
{
    public FluidDetailPage(FluidDetailVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}