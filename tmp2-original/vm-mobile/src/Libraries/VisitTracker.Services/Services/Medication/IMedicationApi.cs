namespace VisitTracker.Services;

public interface IMedicationApi
{
    Task<bool> RequestMedicationAdministration(int medicationId);
}