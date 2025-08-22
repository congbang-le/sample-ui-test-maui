namespace VisitTracker.Services;

public class LocationCentroidApi : ILocationCentroidApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public LocationCentroidApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<LocationCentroid> UpdateGroundTruth(LocationCentroid LocationCentroid)
    {
        return await _requestProvider.ExecuteAsync<LocationCentroid>(
                Constants.EndUrlUpdateGroundTruth, HttpMethod.Post,
                LocationCentroid
            );
    }
}