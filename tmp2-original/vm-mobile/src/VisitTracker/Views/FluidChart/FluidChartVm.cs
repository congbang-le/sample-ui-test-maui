namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
public class FluidChartVm : BaseVm, IQueryAttributable
{
    public int Id { get; set; }
    public FluidChartDto FluidChartDto { get; set; }
    public FluidHistoryDto SelectedFluid { get; set; }

    public FluidChartVm()
    { }

    protected override async Task Init()
    {
        var response = await AppServices.Current.FluidChartService.SyncFluidHistory(Id);
        FluidChartDto = BaseAssembler.BuildFluidChartViewModels(response);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var param))
            Id = Convert.ToInt32(param.ToString());

        await InitCommand.Execute();
    }
}