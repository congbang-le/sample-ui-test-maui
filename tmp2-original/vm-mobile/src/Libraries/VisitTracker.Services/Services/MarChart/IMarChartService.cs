namespace VisitTracker.Services;

public interface IMarChartService
{
    Task<MarChartResponse> SyncMedicationHistoryByServiceUser(int serviceUserId);

    Task<MedicationDetail> SyncMedicationHistoryByMedication(int medicationId);
}