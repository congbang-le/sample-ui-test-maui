namespace VisitTracker.Services;

public class TaskService : BaseService<BookingTask>, ITaskService
{
    private readonly ITaskStorage _storage;

    public TaskService(ITaskStorage taskStorage) : base(taskStorage)
    {
        _storage = taskStorage;
    }

    public async Task<IList<BookingTask>> GetAllByBookingId(int bookingId)
    {
        return await _storage.GetAllByBookingId(bookingId);
    }
}