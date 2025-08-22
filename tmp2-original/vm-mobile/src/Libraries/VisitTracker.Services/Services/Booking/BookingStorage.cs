namespace VisitTracker.Services;

public class BookingStorage : BaseStorage<Booking>, IBookingStorage
{
    private readonly ICodeStorage _codeStorage;

    private Code BookingScheduledCode { get; set; }
    private Code BookingProgressCode { get; set; }

    public BookingStorage(ISecuredKeyProvider keyProvider,
        ICodeStorage codeStorage) : base(keyProvider)
    {
        _codeStorage = codeStorage;
    }

    public async Task<IList<Booking>> GetAllByDate(DateTime date)
    {
        var nextDay = date.AddDays(1);

        if (BookingScheduledCode == null)
            BookingScheduledCode = await _codeStorage.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.SCHEDULED);

        return await Select(q => q.Where(x => x.BookingStatusId == BookingScheduledCode.Id && x.StartTimeTicks >= date.Date.Ticks && x.StartTimeTicks < nextDay.Date.Ticks));
    }

    public async Task<IList<Booking>> GetBookingsOlderByDays(int days)
    {
        var date = DateTimeExtensions.NowNoTimezone().Date.AddDays(-days);
        return await Select(q => q.Where(x => x.EndTimeTicks < date.Ticks));
    }

    public async Task<Booking> GetCurrentBooking()
    {
        var date = DateTimeExtensions.NowNoTimezone();

        if (BookingScheduledCode == null)
            BookingScheduledCode = await _codeStorage.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.SCHEDULED);

        if (BookingProgressCode == null)
            BookingProgressCode = await _codeStorage.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.PROGRESS);

        var allBookings = await Select(q => q.Where(x => x.BookingStatusId == BookingScheduledCode.Id
            && x.CompletionStatusId == BookingProgressCode.Id));
        return allBookings?.MinBy(i => i.StartTime) ?? null;
    }

    public async Task<Booking> GetNextBooking()
    {
        var date = DateTimeExtensions.NowNoTimezone();

        if (BookingScheduledCode == null)
            BookingScheduledCode = await _codeStorage.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.SCHEDULED);

        var allBookings = await Select(q => q.Where(x => x.BookingStatusId == BookingScheduledCode.Id
            && x.CompletionStatusId == null));
        return allBookings?.MinBy(x => x.StartTime) ?? null;
    }

    public async Task<Booking> GetNextBooking(int prevBookingId)
    {
        var date = DateTimeExtensions.NowNoTimezone();

        if (BookingScheduledCode == null)
            BookingScheduledCode = await _codeStorage.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.SCHEDULED);

        var prevBooking = await GetById(prevBookingId);
        var allBookings = await Select(q => q.Where(x => x.BookingStatusId == BookingScheduledCode.Id
            && x.StartTimeTicks > prevBooking.StartTimeTicks));
        return allBookings?.MinBy(x => x.StartTime) ?? null;
    }

    public async Task<DateTime?> GetMinDateByRosterId(int rosterId)
    {
        var allBookings = await Select(q => q.Where(x => x.RosterId != null && x.RosterId == rosterId));
        return allBookings?.MinBy(x => x.StartTime)?.StartTime ?? null;
    }

    public async Task<IList<Booking>> GetScheduledBookingsBetweenDates(DateTime StartDate, DateTime EndDate)
    {
        if (BookingScheduledCode == null)
            BookingScheduledCode = await _codeStorage.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.SCHEDULED);

        var allBookings = await GetAll();
        return allBookings.Where(x => x.BookingStatusId == BookingScheduledCode.Id
            && x.StartTimeTicks < EndDate.Ticks && x.StartTimeTicks >= StartDate.Ticks).ToList();
    }

    public async Task<IList<Booking>> GetPastBookings(int count)
    {
        var allBookings = await GetAll();

        return allBookings
            .Where(args => args.StartTime < DateTimeExtensions.NowNoTimezone()).TakeLast(count).ToList();
    }
}