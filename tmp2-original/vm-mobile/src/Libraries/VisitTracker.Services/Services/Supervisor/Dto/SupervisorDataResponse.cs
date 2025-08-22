namespace VisitTracker.Services;

public class SupervisorDataResponse
{
    public List<Code> Codes { get; set; }
    public List<VisitMessage> VisitMessages { get; set; }
    public List<CareWorker> CareWorkers { get; set; }
    public List<ServiceUser> ServiceUsers { get; set; }
    public List<ServiceUserAddress> ServiceUsersAddresses { get; set; }
    public List<LocationCentroid> Centroids { get; set; }
    public List<ServiceUserFp> ServiceUserFps { get; set; }
}