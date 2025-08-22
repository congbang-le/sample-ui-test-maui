namespace VisitTracker.Services;

public interface IProfileStorage : IBaseStorage<Profile>
{
    Task<Profile> GetLoggedInProfileUser();
}