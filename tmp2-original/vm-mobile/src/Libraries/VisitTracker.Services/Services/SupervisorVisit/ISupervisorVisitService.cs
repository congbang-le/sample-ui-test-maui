namespace VisitTracker.Services;

public interface ISupervisorVisitService : IBaseService<SupervisorVisit>
{
    Task<SupervisorVisit> GetBySuAndSup(int suId, int supId);

    Task<IList<SupervisorVisit>> GetCurrentWeekSupervisors();

    Task<SupervisorVisit> StartVisit(SupervisorVisit SupervisorVisit);

    Task<bool> SubmitVisitReport(SupervisorVisitReportDto dto);

    Task<SupervisorStartVisitResponseDto> CanSupStartVisit(SupervisorCheckStartVisitDto dto);
}