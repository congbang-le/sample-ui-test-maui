namespace VisitTracker.Services;

public class SyncResponse
{
    public List<int> SuccessIds { get; set; }
    public List<int> FailIds { get; set; }
    public string ResponseMetaData { get; set; }
}