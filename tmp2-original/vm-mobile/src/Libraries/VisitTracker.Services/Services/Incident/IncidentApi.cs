namespace VisitTracker.Services;

public class IncidentApi : IIncidentApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public IncidentApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<IList<IncidentResponseDto>> GetAllByServiceUser(int UserId)
    {
        return await _requestProvider.ExecuteAsync<IList<IncidentResponseDto>>(
                Constants.EndUrlIncidentsByServiceUser, HttpMethod.Post, UserId
            );
    }

    public async Task<IList<IncidentResponseDto>> GetAllByCareWorker(int UserId)
    {
        return await _requestProvider.ExecuteAsync<IList<IncidentResponseDto>>(
                Constants.EndUrlIncidentsByCareWorker, HttpMethod.Post, UserId
            );
    }

    public async Task<IncidentDetailResponse> GetIncidentDetail(int incidentId)
    {
        return await _requestProvider.ExecuteAsync<IncidentDetailResponse>(
                Constants.EndUrlIncidentsGetDetail, HttpMethod.Post, incidentId
            );
    }

    public async Task<bool> UploadIncidentReportAdhoc(IncidentAdhocRequest data)
    {
        return await _requestProvider.ExecuteAsync<bool>(
            Constants.EndUrlIncidentReportAdhoc, HttpMethod.Post, data
            );
    }
}