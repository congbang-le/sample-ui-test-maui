namespace VisitTracker.Services;

public class MedicationApi : IMedicationApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public MedicationApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<bool> RequestMedicationAdministration(int medicationId)
    {
        return await _requestProvider.ExecuteAsync<bool>(
                               Constants.EndUrlMedAdminReq, HttpMethod.Post, medicationId.ToString()
                           );
    }
}