namespace VisitTracker;

public partial class HtmlPage : BaseContentPage<HtmlVm>
{
    public HtmlPage(HtmlVm viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
    }
}