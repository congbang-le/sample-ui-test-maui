namespace VisitTracker.Services;

public class FluidChartResponse
{
    public List<VisitFluid> Fluids { get; set; }
    public int? OralIntakeTotal { get; set; }
    public int? IvScIntakeTotal { get; set; }
    public int? OtherIntakeTotal { get; set; }
    public int? UrineOutputTotal { get; set; }
    public int? VomitOutputTotal { get; set; }
    public int? TubeOutputTotal { get; set; }
    public int? OtherOutputTotal { get; set; }
    public int? TodayBalance { get; set; }
    public int? YesterdayBalance { get; set; }
    public string TodayBalanceTime { get; set; }
}