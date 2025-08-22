namespace VisitTracker.Services;

public interface IVisitConsumableService : IBaseService<VisitConsumable>
{
    Task<IList<VisitConsumable>> GetAllByVisitId(int bookingId);

    Task DeleteAllByVisitId(int bookingId);
}