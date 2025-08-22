namespace VisitTracker.Domain;

public class Provider : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; } = new Random().Next();

    public string Name { get; set; }

    public string Address { get; set; }

    public string ImageUrl { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string CookieDomain { get; set; }

    public string Identifier { get; set; }

    public string ServerUrl { get; set; }
}