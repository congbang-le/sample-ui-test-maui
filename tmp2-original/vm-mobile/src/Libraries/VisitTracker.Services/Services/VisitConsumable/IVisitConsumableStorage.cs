namespace VisitTracker.Services;

public interface IVisitConsumableStorage : IBaseStorage<VisitConsumable>
{
    Task<IList<VisitConsumable>> GetAllByVisitId(int bookingId);

    Task DeleteAllByVisitId(int bookingId);
}