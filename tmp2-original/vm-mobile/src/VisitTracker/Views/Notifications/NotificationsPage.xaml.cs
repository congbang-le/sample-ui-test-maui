namespace VisitTracker;

public partial class NotificationsPage : BaseContentPage<NotificationsVm>
{
    public NotificationsPage(NotificationsVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}