namespace VisitTracker.Services;

public interface IBookingDetailStorage : IBaseStorage<BookingDetail>
{
    Task<IList<BookingDetail>> GetAllByBookingId(int id);

    Task<IList<BookingDetail>> GetAllByBookingIds(IEnumerable<int> ids);

    Task<BookingDetail> GetMasterByBookingId(int id);

    Task<BookingDetail> GetByBookingId(int id);

    Task DeleteAllByBookingId(int id);

    Task<BookingDetail> GetBookingDetailForCurrentCw(int bookingId);

    Task<IList<BookingDetail>> GetAllForBooking(int bookingId);
}