namespace VisitTracker.Services;

public interface ISupervisorApi
{
    Task<SupervisorDataResponse> DownloadAll();

    Task<SupervisorFormsResponse> GetFormDetailsBySup(int supId);
}