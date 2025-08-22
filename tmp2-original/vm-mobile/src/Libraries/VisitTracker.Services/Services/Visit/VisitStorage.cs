namespace VisitTracker.Services;

public class VisitStorage : BaseStorage<Visit>, IVisitStorage
{
    private readonly IBookingDetailStorage _bookingDetailStorage;
    private readonly ICareWorkerStorage _careWorkerStorage;

    public VisitStorage(ISecuredKeyProvider keyProvider,
        IBookingDetailStorage bookingDetailStorage,
        ICareWorkerStorage careWorkerStorage) : base(keyProvider)
    {
        _bookingDetailStorage = bookingDetailStorage;
        _careWorkerStorage = careWorkerStorage;
    }

    public async Task<Visit> GetByBookingDetailId(int bookingDetailId)
    {
        return await FirstOrDefault(q => q.Where(x => x.BookingDetailId == bookingDetailId));
    }

    public async Task<Visit> GetByBookingId(int bookingId)
    {
        var bookingDetail = await _bookingDetailStorage.GetMasterByBookingId(bookingId);
        var allVisits = await Select(q => q.Where(x => x.BookingDetailId == bookingDetail.Id));
        return allVisits.MaxBy(x => x.CompletedOn);
    }

    public async Task DeleteAllByBookingId(int bookingId)
    {
        var bookingDetails = await _bookingDetailStorage.GetAllByBookingId(bookingId);
        foreach (var bookingDetail in bookingDetails)
        {
            var bookingVisits = await Select(q => q.Where(x => x.BookingDetailId == bookingDetail.Id));
            if (bookingVisits != null && bookingVisits.Any())
                await DeleteAllByIds(bookingVisits.Select(x => x.Id));
        }
    }

    public async Task<Visit> GetByBookingIdForCurrentCw(int bookingId)
    {
        var bookingDetail = await _bookingDetailStorage.GetBookingDetailForCurrentCw(bookingId);
        return await FirstOrDefault(q => q.Where(x => x.BookingDetailId == bookingDetail.Id));
    }
}