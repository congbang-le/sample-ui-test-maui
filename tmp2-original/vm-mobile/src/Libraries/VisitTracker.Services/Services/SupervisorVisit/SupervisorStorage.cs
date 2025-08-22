namespace VisitTracker.Services;

public class SupervisorVisitStorage : BaseStorage<SupervisorVisit>, ISupervisorVisitStorage
{
    public SupervisorVisitStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<SupervisorVisit> GetBySuAndSup(int suId, int supId)
    {
        return await FirstOrDefault(q => q.Where(x => x.ServiceUserId == suId && x.SupervisorId == supId).OrderByDescending(j => j.StartedOn));
    }
}