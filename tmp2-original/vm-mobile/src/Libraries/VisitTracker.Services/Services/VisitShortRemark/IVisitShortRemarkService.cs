namespace VisitTracker.Services;

public interface IVisitShortRemarkService : IBaseService<VisitShortRemark>
{
    Task<IList<VisitShortRemark>> GetAllByVisitId(int bookingId);

    Task DeleteAllByVisitId(int bookingId);
}