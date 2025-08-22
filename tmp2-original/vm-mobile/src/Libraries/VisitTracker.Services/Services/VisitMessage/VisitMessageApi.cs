namespace VisitTracker.Services;

public class VisitMessageApi : IVisitMessageApi
{
    private readonly IRestServiceRequestProvider _requestProvider;

    public VisitMessageApi(TargetRestServiceRequestProvider requestProvider)
    {
        _requestProvider = requestProvider;
    }

    public async Task<IList<VisitMessage>> SyncVisitMessages()
    {
        return await _requestProvider.ExecuteAsync<IList<VisitMessage>>(
                Constants.EndUrlVisitMessages, HttpMethod.Get
            );
    }
}