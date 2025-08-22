namespace VisitTracker.Services;

public class CodeStorage : BaseStorage<Code>, ICodeStorage
{
    public CodeStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<Code>> GetByType(ECodeType type)
    {
        var allCodes = await GetAll();
        return allCodes.Where(x => x.Type == type.ToString()).ToList();
    }

    public async Task<Code> GetByTypeValue(ECodeType type, ECodeName name)
    {
        var allCodes = await GetAll();
        return allCodes.FirstOrDefault(x => x.Type == type.ToString() && x.Name == name.ToString());
    }
}