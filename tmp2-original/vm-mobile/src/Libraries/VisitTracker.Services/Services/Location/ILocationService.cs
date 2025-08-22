namespace VisitTracker.Services;

public interface ILocationService : IBaseService<VisitLocation>
{
    Task<IList<VisitLocation>> GetAllByServiceUserFp(int serviceUserId);

    Task<IList<VisitLocation>> GetAllUnSyncLocation(int bookingId);

    Task<bool> DeleteAllByVisitEmpty();

    Task<bool> DeleteAllByServiceUserFpEmpty();

    Task<IList<VisitLocation>> GetAllByVisitId(int id);

    Task DeleteAllByVisitId(int bookingId);

    Task<IList<VisitLocation>> GetAllUnSyncSupervisorLocation(int supVisitId);

    Task<bool> DeleteAllBySup(int supVisitId);

    Task<IList<VisitLocation>> GetBySupVisit(int id);
}