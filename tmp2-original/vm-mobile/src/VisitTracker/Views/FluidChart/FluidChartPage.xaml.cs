namespace VisitTracker;

public partial class FluidChartPage : BaseContentPage<FluidChartVm>
{
    public FluidChartPage(FluidChartVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}