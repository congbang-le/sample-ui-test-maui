namespace VisitTracker.Services;

public interface ISupervisorVisitStorage : IBaseStorage<SupervisorVisit>
{
    Task<SupervisorVisit> GetBySuAndSup(int suId, int supId);
}