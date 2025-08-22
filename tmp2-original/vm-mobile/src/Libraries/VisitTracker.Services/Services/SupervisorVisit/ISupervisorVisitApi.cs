namespace VisitTracker.Services;

public interface ISupervisorVisitApi
{
    Task<SupervisorVisit> StartVisit(SupervisorVisit SupervisorVisit);

    Task<bool> SubmitVisitReport(SupervisorVisitReportDto dto);

    Task<SupervisorStartVisitResponseDto> CanSupStartVisit(SupervisorCheckStartVisitDto dto);
}