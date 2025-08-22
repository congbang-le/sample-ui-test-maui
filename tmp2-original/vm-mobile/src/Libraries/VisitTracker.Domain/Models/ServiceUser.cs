namespace VisitTracker.Domain;

public class ServiceUser : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Notes { get; set; }
    public string Address { get; set; }

    [NotMapped]
    public string SignedImageUrl { get; set; }

    public string ImageUrl { get; set; }

    [JsonPropertyName("PhoneNo1")]
    public string Phone { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public bool? iOSFpAvailable { get; set; }
    public bool? AndroidFpAvailable { get; set; }

    public string Gender { get; set; }

    [JsonPropertyName("genderId")]
    public int GenderId { get; set; }
}