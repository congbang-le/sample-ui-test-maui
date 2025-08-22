namespace VisitTracker.Services;

public interface IVisitHealthStatusService : IBaseService<VisitHealthStatus>
{
    Task<IList<VisitHealthStatus>> GetAllByVisitId(int bookingId);

    Task DeleteAllByVisitId(int bookingId);
}