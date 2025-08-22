namespace VisitTracker;

public partial class CareWorkerDetailPage : BaseContentPage<CareWorkerDetailVm>
{
    public CareWorkerDetailPage(CareWorkerDetailVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}