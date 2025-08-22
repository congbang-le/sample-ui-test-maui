namespace VisitTracker.Services;

public class SupervisorVisitService : BaseService<SupervisorVisit>, ISupervisorVisitService
{
    private readonly ISupervisorVisitStorage _storage;
    private readonly ISupervisorVisitApi _api;

    public SupervisorVisitService(ISupervisorVisitStorage SupervisorVisitStorage
        , ISupervisorVisitApi api) : base(SupervisorVisitStorage)
    {
        _storage = SupervisorVisitStorage;
        _api = api;
    }

    public async Task<SupervisorVisit> GetBySuAndSup(int suId, int supId)
    {
        return await _storage.GetBySuAndSup(suId, supId);
    }

    public async Task<IList<SupervisorVisit>> GetCurrentWeekSupervisors()
    {
        DateTime currentDate = DateTimeExtensions.NowNoTimezone().Date;
        DateTime weekStartDate = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek);
        DateTime weekEndDate = weekStartDate.Date.AddDays(6);

        var allSupervisorVisits = await GetAll();
        return allSupervisorVisits.Where(x => x.CompletedOn <= weekEndDate && x.CompletedOn >= weekStartDate).ToList();
    }

    public async Task<SupervisorVisit> StartVisit(SupervisorVisit SupervisorVisit)
    {
        var supVisit = await _api.StartVisit(SupervisorVisit);
        supVisit = await _storage.InsertOrReplace(supVisit);
        return supVisit;
    }

    public async Task<bool> SubmitVisitReport(SupervisorVisitReportDto dto)
    {
        return await _api.SubmitVisitReport(dto);
    }

    public async Task<SupervisorStartVisitResponseDto> CanSupStartVisit(SupervisorCheckStartVisitDto dto)
    {
        return await _api.CanSupStartVisit(dto);
    }
}