namespace VisitTracker.Services;

public interface ITaskService : IBaseService<BookingTask>
{
    Task<IList<BookingTask>> GetAllByBookingId(int bookingId);
}