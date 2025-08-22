namespace VisitTracker.Services;

public interface IIncidentStorage : IBaseStorage<Incident>
{
    Task<IList<Incident>> GetAllAdhoc();

    Task<IList<Incident>> GetAllByVisitId(int id);

    Task<Incident> GetByVisitId(int id);

    Task DeleteAllByBookingId(int id);
}