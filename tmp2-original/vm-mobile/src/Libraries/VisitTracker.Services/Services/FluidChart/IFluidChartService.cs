namespace VisitTracker.Services;

public interface IFluidChartService
{
    Task<FluidChartResponse> SyncFluidHistory(int serviceUserId);
}