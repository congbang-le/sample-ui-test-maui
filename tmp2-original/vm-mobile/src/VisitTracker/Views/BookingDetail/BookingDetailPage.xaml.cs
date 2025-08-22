namespace VisitTracker;

public partial class BookingDetailPage : BaseContentPage<BookingDetailVm>
{
    public BookingDetailPage(BookingDetailVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}