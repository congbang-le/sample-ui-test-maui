namespace VisitTracker.Services;

public interface IFluidChartApi
{
    Task<FluidChartResponse> GetFluidHistoryByServiceUser(int serviceUserId);
}