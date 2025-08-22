namespace VisitTracker.Services;

public class FluidChartApi : IFluidChartApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public FluidChartApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<FluidChartResponse> GetFluidHistoryByServiceUser(int serviceUserId)
    {
        return await _requestProvider.ExecuteAsync<FluidChartResponse>(
                        Constants.EndUrlFluidChart, HttpMethod.Post,
                       serviceUserId.ToString()
                    );
    }
}