namespace VisitTracker.Services;

public interface ITaskStorage : IBaseStorage<BookingTask>
{
    Task<IList<BookingTask>> GetAllByBookingId(int bookingId);
}