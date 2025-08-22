namespace VisitTracker;

public partial class ServiceUserDetailPage : BaseContentPage<ServiceUserDetailVm>
{
    private const int TabViewScrollViewMarginAllowance = 4;

    public ServiceUserDetailPage(ServiceUserDetailVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }

    private void BorderParent_SizeChanged(System.Object sender, System.EventArgs e)
    {
        TabView.TabHeaderItemColumnWidth = (BorderParent.Width - BorderParent.Padding.Left - BorderParent.Padding.Right - TabViewScrollViewMarginAllowance) / 2;
        TabView2.TabHeaderItemColumnWidth = (BorderParent.Width - BorderParent.Padding.Left - BorderParent.Padding.Right - TabViewScrollViewMarginAllowance) / 2;
    }
}