namespace VisitTracker.Services;

public class MarChartService : IMarChartService
{
    private readonly IMarChartApi _api;

    public MarChartService(IMarChartApi api)
    {
        _api = api;
    }

    public async Task<MedicationDetail> SyncMedicationHistoryByMedication(int medicationId)
    {
        return await _api.GetMedicationHistoryByMedication(medicationId);
    }

    public async Task<MarChartResponse> SyncMedicationHistoryByServiceUser(int serviceUserId)
    {
        return await _api.GetMedicationHistoryByServiceUser(serviceUserId);
    }
}