namespace VisitTracker.Services;

public interface IVisitApi
{
    Task<string> CanStartVisit(StartVisitDto dto);

    Task<Visit> SyncVisit(Visit visit, VisitBiometricDto biometricDto = null);

    Task<bool> PingLocation(LastKnownDto dto);

    Task<bool> CanSubmitReport(int bookingId);

    Task<bool> SubmitVisitReport(VisitReportDto dto);

    Task<bool> SubmitVisitEditReport(VisitReportEditDto dto);
}