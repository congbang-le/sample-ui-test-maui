namespace VisitTracker.Services;

public class BookingDetailService : BaseService<BookingDetail>, IBookingDetailService
{
    private readonly IBookingDetailStorage _bookingDetailStorage;

    public BookingDetailService(IBookingDetailStorage bookingDetailStorage) : base(bookingDetailStorage)
    {
        _bookingDetailStorage = bookingDetailStorage;
    }

    public async Task<BookingDetail> GetByBookingId(int id)
    {
        return await _bookingDetailStorage.GetByBookingId(id);
    }

    public async Task<IList<BookingDetail>> GetByBookingIds(IList<int> id)
    {
        return await _bookingDetailStorage.GetAllByBookingIds(id);
    }

    public async Task<BookingDetail> GetBookingDetailForCurrentCw(int bookingId)
    {
        return await _bookingDetailStorage.GetBookingDetailForCurrentCw(bookingId);
    }

    public async Task<IList<BookingDetail>> GetAllForBooking(int bookingId)
    {
        return await _bookingDetailStorage.GetAllForBooking(bookingId);
    }
}