namespace VisitTracker.Services;

public class CodeService : BaseService<Code>, ICodeService
{
    private readonly ICodeStorage _storage;

    public CodeService(ICodeStorage codeStorage) : base(codeStorage)
    {
        _storage = codeStorage;
    }

    public async Task<IList<Code>> GetAllByType(ECodeType type)
    {
        return await _storage.GetByType(type);
    }

    public async Task<Code> GetByTypeValue(ECodeType type, ECodeName name)
    {
        return await _storage.GetByTypeValue(type, name);
    }
}