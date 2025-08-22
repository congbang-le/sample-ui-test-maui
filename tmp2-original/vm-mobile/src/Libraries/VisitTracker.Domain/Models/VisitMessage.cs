namespace VisitTracker.Domain;

public class VisitMessage : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Type { get; set; }

    public string Message { get; set; }
}