namespace VisitTracker;

public partial class MarChartPage : BaseContentPage<MarChartVm>
{
    public MarChartPage(MarChartVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}