namespace VisitTracker;

public partial class ErrorPage : BaseContentPage<ErrorVm>
{
    public ErrorPage(ErrorVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}