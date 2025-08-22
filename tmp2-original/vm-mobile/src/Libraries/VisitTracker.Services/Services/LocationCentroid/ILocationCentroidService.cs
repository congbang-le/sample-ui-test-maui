namespace VisitTracker.Services;

public interface ILocationCentroidService : IBaseService<LocationCentroid>
{
    Task<LocationCentroid> GetGroundTruth(int serviceUserId);

    Task<LocationCentroid> UpdateGroundTruth(LocationCentroid LocationCentroid);
}