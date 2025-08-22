namespace VisitTracker.Services;

public class BookingApi : IBookingApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public BookingApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<BookingsResponse> DownloadRoster(int id)
    {
        return await _requestProvider.ExecuteAsync<BookingsResponse>(
                Constants.EndUrlDownloadRoster, HttpMethod.Post, id
            );
    }

    public async Task<BookingsResponse> GetAllBookings()
    {
        return await _requestProvider.ExecuteAsync<BookingsResponse>(
                Constants.EndUrlBookings, HttpMethod.Get
            );
    }

    public async Task<BookingsResponse> GetBooking(int id)
    {
        return await _requestProvider.ExecuteAsync<BookingsResponse>(
                Constants.EndUrlBooking, HttpMethod.Post, id
            );
    }

    public async Task<BookingsResponse> GetAllBySuAndDate(int suId, DateTime date)
    {
        return await _requestProvider.ExecuteAsync<BookingsResponse>(
                Constants.EndUrlBookingsBySuAndDate, HttpMethod.Post,
               new { Date = date, UserId = suId }
            );
    }

    public async Task<BookingsResponse> GetAllByCwAndDate(int cwId, DateTime date)
    {
        return await _requestProvider.ExecuteAsync<BookingsResponse>(
                Constants.EndUrlBookingsByCwAndDate, HttpMethod.Post,
               new { Date = date, UserId = cwId }
            );
    }

    public async Task<BookingsResponse> GetSuBookings(int serviceUserId)
    {
        return await _requestProvider.ExecuteAsync<BookingsResponse>(
                Constants.EndUrlSuBookings, HttpMethod.Post,
                serviceUserId
            );
    }

    public async Task<BookingEditResponse> CheckBookingEditAccess(int bookingDetailId)
    {
        return await _requestProvider.ExecuteAsync<BookingEditResponse>(
                Constants.EndUrlBookingEditAccess, HttpMethod.Post,
                bookingDetailId
            );
    }

    public async Task<bool?> CheckMasterCwChange(int bookingDetailId)
    {
        return await _requestProvider.ExecuteAsync<bool>(
                Constants.EndUrlCheckMasterCwChange, HttpMethod.Post,
                bookingDetailId
            );
    }

    public async Task<string> GetHandOverNotes(int bookingId)
    {
        return await _requestProvider.ExecuteAsync<string>(
                Constants.EndUrlGetHandOverNotes, HttpMethod.Post,
                bookingId
            );
    }

    public async Task<BookingsResponse> DownloadVisits(int bookingId)
    {
        return await _requestProvider.ExecuteAsync<BookingsResponse>(
            Constants.EndUrlDownloadVisits, HttpMethod.Post,
            bookingId
        );
    }
}