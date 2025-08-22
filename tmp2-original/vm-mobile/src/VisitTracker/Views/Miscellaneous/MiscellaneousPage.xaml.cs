namespace VisitTracker;

public partial class MiscellaneousPage : BaseContentPage<MiscellaneousVm>
{
    public MiscellaneousPage(MiscellaneousVm viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}