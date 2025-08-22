namespace VisitTracker;

public class LoaderVm : BaseVm
{
    protected override async Task Init() => await Task.Delay(200);
}