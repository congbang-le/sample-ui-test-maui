namespace VisitTracker.Services;

public interface ICodeService : IBaseService<Code>
{
    Task<IList<Code>> GetAllByType(ECodeType type);

    Task<Code> GetByTypeValue(ECodeType type, ECodeName name);
}