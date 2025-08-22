namespace VisitTracker.Services;

public interface IIncidentApi
{
    Task<IList<IncidentResponseDto>> GetAllByServiceUser(int suId);

    Task<IList<IncidentResponseDto>> GetAllByCareWorker(int cwId);

    Task<IncidentDetailResponse> GetIncidentDetail(int incidentId);

    Task<bool> UploadIncidentReportAdhoc(IncidentAdhocRequest incident);
}