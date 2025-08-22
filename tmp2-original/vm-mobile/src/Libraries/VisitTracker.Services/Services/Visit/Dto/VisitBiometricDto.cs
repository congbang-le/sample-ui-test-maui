namespace VisitTracker.Services;

public class VisitBiometricDto
{
    public bool IsRegistrationSucess { get; set; }
    public bool? HasNewEnrolments { get; set; }
    public string HardwareStatus { get; set; }
    public string ResponseStatus { get; set; }
    public string AuthenticationType { get; set; }
    public string ErrorMessage { get; set; }
}