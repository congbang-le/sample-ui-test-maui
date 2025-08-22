namespace VisitTracker.Services;

public class SupervisorVisitApi : ISupervisorVisitApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public SupervisorVisitApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<SupervisorVisit> StartVisit(SupervisorVisit SupervisorVisit)
    {
        return await _requestProvider.ExecuteAsync<SupervisorVisit>(
            Constants.EndUrlSupStartVisit, HttpMethod.Post,
            SupervisorVisit
        );
    }

    public async Task<bool> SubmitVisitReport(SupervisorVisitReportDto dto)
    {
        return await _requestProvider.ExecuteAsync<bool>(
            Constants.EndUrlSupSubmitVisitReport, HttpMethod.Post,
            dto
        );
    }

    public async Task<SupervisorStartVisitResponseDto> CanSupStartVisit(SupervisorCheckStartVisitDto dto)
    {
        return await _requestProvider.ExecuteAsync<SupervisorStartVisitResponseDto>(
                Constants.EndUrlSupStartVisitCheck, HttpMethod.Post,
                dto
            );
    }
}