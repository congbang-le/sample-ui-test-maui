namespace VisitTracker.Services;

public interface IBookingDetailService : IBaseService<BookingDetail>
{
    Task<BookingDetail> GetByBookingId(int id);

    Task<IList<BookingDetail>> GetByBookingIds(IList<int> ids);

    Task<BookingDetail> GetBookingDetailForCurrentCw(int bookingId);

    Task<IList<BookingDetail>> GetAllForBooking(int bookingId);
}