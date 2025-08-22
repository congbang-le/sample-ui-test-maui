namespace VisitTracker.Services;

public class VisitApi : IVisitApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public VisitApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<string> CanStartVisit(StartVisitDto dto)
    {
        return await _requestProvider.ExecuteAsync<string>(
            Constants.EndUrlCanStartVisit, HttpMethod.Post,
            dto
        );
    }

    public async Task<Visit> SyncVisit(Visit visit, VisitBiometricDto biometricDto = null)
    {
        return await _requestProvider.ExecuteAsync<Visit>(
            Constants.EndUrlSyncVisit, HttpMethod.Post,
            new { Visit = visit, VisitBiometric = biometricDto }
        );
    }

    public async Task<bool> PingLocation(LastKnownDto dto)
    {
        return await _requestProvider.ExecuteAsync<bool>(
            Constants.EndUrlPingLocation, HttpMethod.Post,
            dto
        );
    }

    public async Task<bool> CanSubmitReport(int bookingId)
    {
        return await _requestProvider.ExecuteAsync<bool>(
            Constants.EndUrlCanSubmitReport, HttpMethod.Post,
            bookingId
        );
    }

    public async Task<bool> SubmitVisitReport(VisitReportDto dto)
    {
        return await _requestProvider.ExecuteAsync<bool>(
            Constants.EndUrlSubmitVisitReport, HttpMethod.Post,
            dto
        );
    }

    public async Task<bool> SubmitVisitEditReport(VisitReportEditDto dto)
    {
        return await _requestProvider.ExecuteAsync<bool>(
            Constants.EndUrlSubmitVisitEditReport, HttpMethod.Post,
            dto
        );
    }
}