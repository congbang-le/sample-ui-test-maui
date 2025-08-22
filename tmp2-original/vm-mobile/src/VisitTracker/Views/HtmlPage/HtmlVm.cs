namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
public class HtmlVm : BaseVm, IQueryAttributable
{
    public int Id { get; set; }
    public WebViewSource Source { get; set; }

    public HtmlVm()
    { }

    protected override async Task Init()
    {
        var notification = await AppServices.Current.NotificationService.GetById(Id);
        Source = new HtmlWebViewSource { Html = notification.Content };
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var id))
            Id = Convert.ToInt32(id);

        await InitCommand.Execute();
    }
}