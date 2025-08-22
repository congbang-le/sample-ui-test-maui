namespace VisitTracker.Services;

public class StepCountStorage : BaseStorage<VisitStepCount>, IStepCountStorage
{
    private readonly IBookingDetailStorage _bookingDetailStorage;

    public StepCountStorage(ISecuredKeyProvider keyProvider
        , IBookingDetailStorage bookingDetailStorage) : base(keyProvider)
    {
        _bookingDetailStorage = bookingDetailStorage;
    }

    public async Task<IList<VisitStepCount>> GetAllByVisitId(int id)
    {
        return await Select(q => q.Where(x => x.VisitId == id));
    }

    public async Task DeleteAllByVisitId(int bookingId)
    {
        var bookingDetails = await _bookingDetailStorage.GetAllByBookingId(bookingId);
        foreach (var bookingDetail in bookingDetails)
        {
            var stepcounts = await Select(q => q.Where(x => x.VisitId == bookingDetail.Id));
            if (stepcounts != null && stepcounts.Any())
                await DeleteAllByIds(stepcounts.Select(x => x.Id));
        }
    }

    public async Task<IList<VisitStepCount>> GetAllBySupVisitId(int id)
    {
        return await Select(q => q.Where(x => x.SupervisorVisitId == id));
    }
}