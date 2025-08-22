namespace VisitTracker.Services;

public interface ISupervisorService
{
    Task<bool> SyncData();

    Task<SupervisorFormsResponse> GetFormDetailsBySup(int supId);
}