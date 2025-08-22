namespace VisitTracker.Services;

public interface IVisitShortRemarkStorage : IBaseStorage<VisitShortRemark>
{
    Task<IList<VisitShortRemark>> GetAllByVisitId(int bookingId);

    Task DeleteAllByVisitId(int bookingId);
}