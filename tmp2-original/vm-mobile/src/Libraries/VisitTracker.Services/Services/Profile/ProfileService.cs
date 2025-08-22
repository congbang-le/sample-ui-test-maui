namespace VisitTracker.Services;

public class ProfileService : BaseService<Profile>, IProfileService
{
    private readonly IProfileStorage _storage;

    public ProfileService(IProfileStorage ProfileStorage) : base(ProfileStorage)
    {
        _storage = ProfileStorage;
    }

    public async Task<Profile> GetLoggedInProfileUser()
    {
        return await _storage.GetLoggedInProfileUser();
    }
}