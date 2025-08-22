namespace VisitTracker.Services;

public interface IBodyMapService : IBaseService<BodyMap>
{
    Task<IList<BodyMap>> GetAllByBaseVisitId(int id);

    Task<IList<BodyMap>> GetAllByVisitId(int bookingId);

    Task<IList<BodyMap>> GetAllByVisitTaskId(int taskId);

    Task<IList<BodyMap>> GetAllByVisitMedicationId(int medicationId);

    Task<IList<BodyMap>> GetAllByFluidId(int fluidId);

    Task<IList<BodyMap>> GetAllByIncidentId(int incidentId);

    Task<BodyMap> GetByBookingId(int bookingId);

    Task<BodyMap> GetByTaskId(int taskId);

    Task<BodyMap> GetByMedicationId(int medicationId);

    Task<BodyMap> GetByFluidId(int fluidId);

    Task<BodyMap> GetByIncidentId(int incidentId);

    Task DeleteAllByVisitId(int visitId);

    Task DeleteAllByIncidentId(int incidentId);
}