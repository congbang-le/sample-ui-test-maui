namespace VisitTracker;

public partial class BookingsPage : BaseContentPage<BookingsVm>
{
    public BookingsPage(BookingsVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}