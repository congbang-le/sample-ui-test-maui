namespace VisitTracker.Services;

public class AttachmentStorage : BaseStorage<Attachment>, IAttachmentStorage
{
    public AttachmentStorage(ISecuredKeyProvider keyProvider) : base(keyProvider)
    { }

    public async Task<IList<Attachment>> GetAllByBaseVisitId(int id)
    {
        return await Select(q => q.Where(x => x.BaseVisitId == id));
    }

    public async Task<IList<Attachment>> GetAllByVisitId(int id)
    {
        return await Select(q => q.Where(x => x.VisitId == id));
    }

    public async Task<IList<Attachment>> GetByTaskId(int id)
    {
        return await Select(q => q.Where(x => x.VisitTaskId == id));
    }

    public async Task<IList<Attachment>> GetByMedicationId(int id)
    {
        return await Select(q => q.Where(x => x.VisitMedicationId == id));
    }

    public async Task<IList<Attachment>> GetByFluidId(int id)
    {
        return await Select(q => q.Where(x => x.VisitFluidId == id));
    }

    public async Task<IList<Attachment>> GetByIncidentId(int id)
    {
        return await Select(q => q.Where(x => x.IncidentId == id));
    }

    public async Task<IList<Attachment>> GetByBodyMapId(int id)
    {
        if (id > 0) return await Select(q => q.Where(x => x.BodyMapId == id));
        return default;
    }

    public async Task DeleteAllByVisitId(int id)
    {
        var attachments = await Select(q => q.Where(x => x.VisitId == id));
        if (attachments != null && attachments.Any())
            await DeleteAllByIds(attachments.Select(x => x.Id));
    }

    public async Task DeleteAllByBodyMapId(int id)
    {
        var attachments = await Select(q => q.Where(x => x.BodyMapId == id));
        if (attachments != null && attachments.Any())
            await DeleteAllByIds(attachments.Select(x => x.Id));
    }

    public async Task DeleteAllByIncidentId(int id)
    {
        var attachments = await Select(q => q.Where(x => x.IncidentId == id));
        if (attachments != null && attachments.Any())
            await DeleteAllByIds(attachments.Select(x => x.Id));
    }
}