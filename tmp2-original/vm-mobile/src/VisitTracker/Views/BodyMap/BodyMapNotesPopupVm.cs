namespace VisitTracker;

public class BodyMapNotesPopupVm : BaseVm
{
    public int Id { get; set; }
    public int? BaseVisitId { get; set; }

    public string Summary { get; set; } = null;
    public string CompletedOn { get; set; }
    public string Parts { get; set; } = null;
    public IList<AttachmentDto> AttachmentList { get; set; }
    public bool IsAllowToSave { get; set; } = false;
    public bool IsAllowToDiscard { get; set; } = false;
    public string TypeName => EPageType.BodyMapType.ToString();

    public double popupWidth { get; set; }

    protected override async Task Init() => await Task.Delay(200);
}