namespace VisitTracker.Services;

public interface IBookingApi
{
    Task<BookingsResponse> DownloadRoster(int rosterId);

    Task<BookingsResponse> GetAllBookings();

    Task<BookingsResponse> GetBooking(int bookingId);

    Task<BookingsResponse> GetAllBySuAndDate(int suId, DateTime Date);

    Task<BookingsResponse> GetAllByCwAndDate(int cwId, DateTime Date);

    Task<BookingsResponse> GetSuBookings(int serviceUserId);

    Task<BookingEditResponse> CheckBookingEditAccess(int bookingDetailId);

    Task<bool?> CheckMasterCwChange(int bookingDetailId);

    Task<string> GetHandOverNotes(int bookingId);

    Task<BookingsResponse> DownloadVisits(int bookingId);
}