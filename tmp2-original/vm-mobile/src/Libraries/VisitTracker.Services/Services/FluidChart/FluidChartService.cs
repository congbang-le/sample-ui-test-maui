namespace VisitTracker.Services;

public class FluidChartService : IFluidChartService
{
    private readonly IFluidChartApi _api;

    public FluidChartService(IFluidChartApi api)
    {
        _api = api;
    }

    public async Task<FluidChartResponse> SyncFluidHistory(int serviceUserId)
    {
        return await _api.GetFluidHistoryByServiceUser(serviceUserId);
    }
}