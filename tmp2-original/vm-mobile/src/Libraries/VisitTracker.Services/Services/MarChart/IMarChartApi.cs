namespace VisitTracker.Services;

public interface IMarChartApi
{
    Task<MarChartResponse> GetMedicationHistoryByServiceUser(int serviceUserId);

    Task<MedicationDetail> GetMedicationHistoryByMedication(int medicationId);
}