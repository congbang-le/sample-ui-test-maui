namespace VisitTracker.Services;

public class IncidentResponseDto
{
    public int Id { get; set; }
    public string IncidentTime { get; set; }

    public string SubmittedByType { get; set; }
    public string SubmittedByName { get; set; }

    public string ServiceUserName { get; set; }
    public string ServiceUserImageUrl { get; set; }
}