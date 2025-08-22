namespace VisitTracker;

public partial class BookingEditPage : BaseContentPage<BookingEditVm>
{
    public BookingEditPage(BookingEditVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}