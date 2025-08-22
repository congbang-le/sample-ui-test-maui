namespace VisitTracker.Services;

public interface IBookingStorage : IBaseStorage<Booking>
{
    Task<IList<Booking>> GetAllByDate(DateTime date);

    Task<IList<Booking>> GetBookingsOlderByDays(int days);

    Task<Booking> GetCurrentBooking();

    Task<Booking> GetNextBooking();

    Task<Booking> GetNextBooking(int prevBookingId);

    Task<DateTime?> GetMinDateByRosterId(int rosterId);

    Task<IList<Booking>> GetScheduledBookingsBetweenDates(DateTime StartDate, DateTime EndDate);

    Task<IList<Booking>> GetPastBookings(int count);
}