namespace VisitTracker.Services;

public interface ICodeStorage : IBaseStorage<Code>
{
    Task<IList<Code>> GetByType(ECodeType type);

    Task<Code> GetByTypeValue(ECodeType type, ECodeName name);
}