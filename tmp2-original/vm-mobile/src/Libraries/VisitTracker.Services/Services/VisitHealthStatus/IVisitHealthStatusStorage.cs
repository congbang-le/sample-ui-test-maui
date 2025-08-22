namespace VisitTracker.Services;

public interface IVisitHealthStatusStorage : IBaseStorage<VisitHealthStatus>
{
    Task<IList<VisitHealthStatus>> GetAllByVisitId(int bookingId);

    Task DeleteAllByVisitId(int bookingId);
}