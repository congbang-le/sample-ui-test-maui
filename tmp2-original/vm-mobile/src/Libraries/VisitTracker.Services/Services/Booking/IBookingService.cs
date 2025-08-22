namespace VisitTracker.Services;

public interface IBookingService : IBaseService<Booking>
{
    Task<bool> DownloadRoster(int rosterId);

    Task<bool> SyncAllBookings();

    Task<IList<Booking>> GetAllBookingsByDate(DateTime date);

    Task<IList<Booking>> GetBookingsOlderByDays(int days);

    Task<Booking> GetCurrentBooking();

    Task<Booking> SetCurrentBookingStatus(int bookingId, ECodeType type, ECodeName name);

    Task<Booking> GetNextBooking();

    Task<Booking> GetNextBooking(int prevBookingId);

    Task<DateTime?> GetMinDateByRosterId(int rosterId);

    Task<IList<Booking>> GetScheduledBookingsBetweenDates(DateTime StartDate, DateTime EndDate);

    Task<bool> SyncBookingById(int bookingId);

    Task DownloadVisitsByBookingId(int bookingId);

    Task<IList<Booking>> GetPastBookings(int count);

    Task<IList<Booking>> GetAllBySuAndDate(int suId, DateTime date);

    Task<IList<Booking>> GetAllByCwAndDate(int cwId, DateTime date);

    Task<BookingEditResponse> CheckBookingEditAccess(int bookingId);

    Task<bool?> CheckMasterCwChange(int bookingDetailId);

    Task<string> UpdateHandOverNotes(int bookingId);

    Task DeleteCompleteBookings(IEnumerable<int> bookingIds);

    Task<List<Booking>> PersistBookings(BookingsResponse bookings);
}