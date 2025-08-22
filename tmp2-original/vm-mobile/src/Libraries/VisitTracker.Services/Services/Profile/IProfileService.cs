namespace VisitTracker.Services;

public interface IProfileService : IBaseService<Profile>
{
    Task<Profile> GetLoggedInProfileUser();
}