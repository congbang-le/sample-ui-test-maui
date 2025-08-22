namespace VisitTracker.Services;

public class ProfileStorage : BaseStorage<Profile>, IProfileStorage
{
    public ProfileStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<Profile> GetLoggedInProfileUser()
    {
        var dbProfiles = await GetAll();
        return dbProfiles.FirstOrDefault();
    }
}