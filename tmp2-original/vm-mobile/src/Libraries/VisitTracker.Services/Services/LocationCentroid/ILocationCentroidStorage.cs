namespace VisitTracker.Services;

public interface ILocationCentroidStorage : IBaseStorage<LocationCentroid>
{
    Task<LocationCentroid> GetGroundTruth(int id);
}