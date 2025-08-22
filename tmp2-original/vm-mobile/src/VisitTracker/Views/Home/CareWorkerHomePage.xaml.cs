namespace VisitTracker;

public partial class CareWorkerHomePage : ReactiveShell<CareWorkerHomeVm>
{
    public CareWorkerHomePage(CareWorkerHomeVm viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}