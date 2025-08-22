namespace VisitTracker.Services;

public class ServiceUserResponse
{
    public ServiceUser ServiceUser { get; set; }
    public IList<LocationCentroid> Centroids { get; set; }
    public IList<ServiceUserAddress> ServiceUserAddresses { get; set; }
}