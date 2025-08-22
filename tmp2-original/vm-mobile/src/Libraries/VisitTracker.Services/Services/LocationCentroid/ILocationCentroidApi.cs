namespace VisitTracker.Services;

public interface ILocationCentroidApi
{
    Task<LocationCentroid> UpdateGroundTruth(LocationCentroid LocationCentroid);
}