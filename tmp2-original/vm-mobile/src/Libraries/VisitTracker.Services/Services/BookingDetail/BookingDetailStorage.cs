namespace VisitTracker.Services;

public class BookingDetailStorage : BaseStorage<BookingDetail>, IBookingDetailStorage
{
    private readonly IProfileStorage _profileService;

    public BookingDetailStorage(ISecuredKeyProvider keyProvider,
        IProfileStorage profileService) : base(keyProvider)
    {
        _profileService = profileService;
    }

    public async Task<IList<BookingDetail>> GetAllByBookingId(int id)
    {
        var allBookingDetails = await GetAll();
        var result = allBookingDetails
            .Where(b => b.BookingId == id)
            .ToList();

        return result;
    }

    public async Task<IList<BookingDetail>> GetAllByBookingIds(IEnumerable<int> ids)
    {
        var allBookingDetails = await GetAll();
        var result = allBookingDetails
            .Where(b => ids.Contains(b.BookingId))
            .ToList();

        return result;
    }

    public async Task<BookingDetail> GetMasterByBookingId(int id)
    {
        var bookingDetails = await GetAllByBookingId(id);
        return bookingDetails.FirstOrDefault(b => b.IsMaster);
    }

    public async Task<BookingDetail> GetByBookingId(int id)
    {
        var allBookingDetails = await GetAll();
        var result = allBookingDetails
            .Where(b => b.BookingId == id)
            .FirstOrDefault();

        return result;
    }

    public async Task DeleteAllByBookingId(int id)
    {
        var bookingDetails = await Select(q => q.Where(x => x.BookingId == id));
        if (bookingDetails != null && bookingDetails.Any())
            await DeleteAllByIds(bookingDetails.Select(i => i.Id));
    }

    public async Task<BookingDetail> GetBookingDetailForCurrentCw(int bookingId)
    {
        var currentLoggedInCw = await _profileService.GetLoggedInProfileUser();
        return await FirstOrDefault(q => q.Where(x => x.BookingId == bookingId && x.CareWorkerId == currentLoggedInCw.Id));
    }

    public async Task<IList<BookingDetail>> GetAllForBooking(int bookingId)
    {
        return await Select(q => q.Where(x => x.BookingId == bookingId));
    }
}