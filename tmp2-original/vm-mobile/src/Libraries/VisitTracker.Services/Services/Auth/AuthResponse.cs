namespace VisitTracker.Services;

public class AuthResponse
{
    public string Token { get; set; }

    public DateTime TokenValidTo { get; set; }

    public string RefreshToken { get; set; }

    public DateTime RefreshTokenValidTo { get; set; }
}

public class TpAuthResponse
{
    public string Token { get; set; }

    public DateTime TokenValidTo { get; set; }
}