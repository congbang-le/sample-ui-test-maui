namespace VisitTracker;

public partial class MiscellaneousDetailPage : BaseContentPage<MiscellaneousDetailVm>
{
    public MiscellaneousDetailPage(MiscellaneousDetailVm viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}