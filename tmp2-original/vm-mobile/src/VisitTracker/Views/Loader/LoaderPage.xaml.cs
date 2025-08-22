namespace VisitTracker;

public partial class LoaderPage : BaseContentPage<LoaderVm>
{
    public LoaderPage(LoaderVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}