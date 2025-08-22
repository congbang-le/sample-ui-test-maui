namespace VisitTracker.Services;

public class MarChartApi : IMarChartApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public MarChartApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<MarChartResponse> GetMedicationHistoryByServiceUser(int serviceUserId)
    {
        return await _requestProvider.ExecuteAsync<MarChartResponse>(
                       Constants.EndUrlMarChart, HttpMethod.Post,
                      serviceUserId.ToString()
                   );
    }

    public async Task<MedicationDetail> GetMedicationHistoryByMedication(int medicationId)
    {
        return await _requestProvider.ExecuteAsync<MedicationDetail>(
                       Constants.EndUrlMed, HttpMethod.Post,
                      medicationId.ToString()
                   );
    }
}