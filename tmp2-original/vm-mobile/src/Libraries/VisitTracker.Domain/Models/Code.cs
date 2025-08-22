namespace VisitTracker.Domain;

public class Code : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Type { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public string Color { get; set; }
}