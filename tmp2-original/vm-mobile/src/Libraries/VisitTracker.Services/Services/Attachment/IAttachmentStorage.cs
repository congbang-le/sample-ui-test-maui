namespace VisitTracker.Services;

public interface IAttachmentStorage : IBaseStorage<Attachment>
{
    Task<IList<Attachment>> GetAllByBaseVisitId(int id);

    Task<IList<Attachment>> GetAllByVisitId(int id);

    Task<IList<Attachment>> GetByTaskId(int id);

    Task<IList<Attachment>> GetByMedicationId(int id);

    Task<IList<Attachment>> GetByFluidId(int id);

    Task<IList<Attachment>> GetByIncidentId(int id);

    Task<IList<Attachment>> GetByBodyMapId(int id);

    Task DeleteAllByVisitId(int id);

    Task DeleteAllByBodyMapId(int id);

    Task DeleteAllByIncidentId(int id);
}