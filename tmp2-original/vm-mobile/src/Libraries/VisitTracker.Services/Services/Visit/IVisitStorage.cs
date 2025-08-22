namespace VisitTracker.Services;

public interface IVisitStorage : IBaseStorage<Visit>
{
    Task<Visit> GetByBookingDetailId(int bookingDetailId);

    Task DeleteAllByBookingId(int bookingId);

    Task<Visit> GetByBookingId(int bookingId);

    Task<Visit> GetByBookingIdForCurrentCw(int bookingId);
}