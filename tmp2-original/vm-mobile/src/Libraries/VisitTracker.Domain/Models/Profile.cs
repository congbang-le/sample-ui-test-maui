namespace VisitTracker.Domain;

public class Profile : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Type { get; set; }

    public string UniqueId { get; set; }

    public string Name { get; set; }

    public string UserName { get; set; }

    public string ImageUrl { get; set; }

    public string SignedImageUrl { get; set; }

    public string Token { get; set; }

    public string RefreshToken { get; set; }

    [JsonIgnore]
    public long TokenValidToTicks { get; set; }

    [Ignored]
    public DateTime? TokenValidTo
    {
        get { return TokenValidToTicks != 0 ? new DateTime(TokenValidToTicks) : null; }
        set { TokenValidToTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    [JsonIgnore]
    public long RefreshTokenValidToTicks { get; set; }

    [Ignored]
    public DateTime? RefreshTokenValidTo
    {
        get { return RefreshTokenValidToTicks != 0 ? new DateTime(RefreshTokenValidToTicks) : null; }
        set { RefreshTokenValidToTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    public string ThirdPartyToken { get; set; }

    [JsonIgnore]
    public long ThirdPartyTokenToTicks { get; set; }

    [Ignored]
    public DateTime? ThirdPartyTokenValidTo
    {
        get { return ThirdPartyTokenToTicks != 0 ? new DateTime(ThirdPartyTokenToTicks) : null; }
        set { ThirdPartyTokenToTicks = value.HasValue ? value.Value.Ticks : default; }
    }

    public string Role { get; set; }
    public string LastLogin { get; set; }
    public string IsSuperUser { get; set; }
    public string IsFormManager { get; set; }

    public bool NeedDeviceRegistration { get; set; }
}