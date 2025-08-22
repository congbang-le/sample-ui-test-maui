namespace VisitTracker.Services;

public interface ILocationStorage : IBaseStorage<VisitLocation>
{
    Task<IList<VisitLocation>> GetAllByServiceUserFp(int id);

    Task<IList<VisitLocation>> GetAllUnSyncByBookingDetail(int id);

    Task<bool> DeleteAllByVisitEmpty();

    Task<bool> DeleteAllByServiceUserFpEmpty();

    Task<IList<VisitLocation>> GetAllByVisitId(int id);

    Task DeleteAllByVisitId(int bookingId);

    Task<IList<VisitLocation>> GetAllUnSyncSupLocation(int supVisitId);

    Task<bool> DeleteAllBySup(int supVisitId);

    Task<IList<VisitLocation>> GetBySupVisit(int id);
}