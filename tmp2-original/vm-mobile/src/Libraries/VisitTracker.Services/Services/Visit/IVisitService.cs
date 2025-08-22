namespace VisitTracker.Services;

public interface IVisitService : IBaseService<Visit>
{
    Task<Visit> GetByBookingDetailId(int bookingDetailId);

    Task<IList<Visit>> GetCurrentWeekBookings();

    Task DeleteAllByBookingId(int bookingId);

    Task<Visit> GetByBookingId(int bookingId);

    Task<Visit> GetByBookingIdForCurrentCw(int bookingId);

    Task DeleteVisitsByBookingId(int bookingId);

    Task DeleteVisitsByIds(IEnumerable<int> visitIds);

    Task<VisitDto> GetVisitDtoByBookingId(int bookingId);

    Task<string> CanStartVisit(StartVisitDto dto);

    Task<Visit> SyncVisit(Visit visit, VisitBiometricDto biometricDto = null);

    Task<bool> PingLocation(LastKnownDto dto);

    Task<bool> CanSubmitReport(int bookingId);

    Task<bool> SubmitVisitReport(VisitReportDto dto);

    Task<bool> SubmitVisitEditReport(VisitReportEditDto dto);
}