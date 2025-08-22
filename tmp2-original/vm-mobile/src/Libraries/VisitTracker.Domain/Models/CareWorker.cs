namespace VisitTracker.Domain;

public class CareWorker : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Name { get; set; }

    [NotMapped]
    public string SignedImageUrl { get; set; }

    public string ImageUrl { get; set; }

    public string Email { get; set; }
    public string PhoneNo => string.IsNullOrEmpty(PhoneNo1) ? null : PhoneNo1 ?? PhoneNo2;
    public string PhoneNo1 { get; set; }
    public string PhoneNo2 { get; set; }

    public int GenderId { get; set; }
    public int TransportTypeId { get; set; }

    [NotMapped]
    public bool IsMaster { get; set; }
}