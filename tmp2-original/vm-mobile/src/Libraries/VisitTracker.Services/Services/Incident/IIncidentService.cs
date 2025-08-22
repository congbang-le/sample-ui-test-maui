namespace VisitTracker.Services;

public interface IIncidentService : IBaseService<Incident>
{
    Task<IList<Incident>> GetAllAdhoc();

    Task DeleteAdhocIncidents();

    Task<IList<Incident>> GetAllByVisitId(int bookingId);

    Task<IList<IncidentResponseDto>> GetAllByServiceUser(int suId);

    Task<IList<IncidentResponseDto>> GetAllByCareWorker(int cwId);

    Task<IncidentDetailResponse> GetIncidentDetail(int incidentId);

    Task<bool> UploadIncidentReportAdhoc(IncidentAdhocRequest incident);
}