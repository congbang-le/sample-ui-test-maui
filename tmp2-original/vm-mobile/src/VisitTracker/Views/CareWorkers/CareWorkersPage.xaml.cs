namespace VisitTracker;

public partial class CareWorkersPage : BaseContentPage<CareWorkersVm>
{
    public CareWorkersPage(CareWorkersVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}