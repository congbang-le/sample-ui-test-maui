namespace VisitTracker.Services;

public class BookingService : BaseService<Booking>, IBookingService
{
    private readonly IBookingApi _api;
    private readonly IVisitService _visitService;
    private readonly IBookingStorage _storage;
    private readonly IFileSystemService _fileSystemService;
    private readonly ICodeStorage _codeStorage;
    private readonly ITaskStorage _taskStorage;
    private readonly IVisitMessageStorage _visitMessageStorage;
    private readonly IMedicationStorage _medicationStorage;
    private readonly IMedicationTransactionStorage _medicationTransactionStorage;
    private readonly ICareWorkerStorage _careWorkerStorage;
    private readonly ICareWorkerService _careWorkerService;
    private readonly IServiceUserStorage _serviceUserStorage;
    private readonly IServiceUserService _serviceUserService;
    private readonly IServiceUserAddressStorage _serviceUserAddressStorage;
    private readonly ILocationCentroidStorage _locationCentroidStorage;
    private readonly IBookingDetailStorage _bookingDetailStorage;
    private readonly IVisitStorage _visitStorage;
    private readonly IVisitTaskStorage _visitTaskStorage;
    private readonly IVisitMedicationStorage _visitMedicationStorage;
    private readonly IVisitFluidStorage _visitFluidStorage;
    private readonly IVisitShortRemarkStorage _visitShortRemarkStorage;
    private readonly IVisitHealthStatusStorage _visitHealthStatusStorage;
    private readonly IVisitConsumableStorage _visitConsumableStorage;
    private readonly IIncidentStorage _incidentStorage;
    private readonly IBodyMapStorage _bodyMapStorage;
    private readonly IAttachmentStorage _attachmentStorage;

    public BookingService(IBookingApi api,
        IVisitService visitService,
        ICodeStorage codeStorage,
        IFileSystemService fileSystemService,
        IBookingDetailStorage bookingDetailStorage,
        IVisitStorage bookingVisitStorage,
        IVisitMessageStorage visitMessageStorage,
        IBookingStorage bookingStorage,
        ITaskStorage taskStorage,
        IVisitTaskStorage visitTaskStorage,
        IMedicationStorage medicationStorage,
        IVisitMedicationStorage visitMedicationStorage,
        IVisitShortRemarkStorage visitShortRemarkStorage,
        IVisitHealthStatusStorage visitHealthStatusStorage,
        IMedicationTransactionStorage medicationTransactionStorage,
        IVisitFluidStorage fluidStorage,
        IIncidentStorage incidentStorage,
        IBodyMapStorage bodyMapStorage,
        IAttachmentStorage attachmentStorage,
        ICareWorkerStorage careWorkerStorage,
        ICareWorkerService careWorkerService,
        IServiceUserStorage serviceUserStorage,
        IServiceUserService serviceUserService,
        IServiceUserAddressStorage serviceUserAddressStorage,
        ILocationCentroidStorage locationCentroidStorage,
        IVisitConsumableStorage bookingConsumableStorage) : base(bookingStorage)
    {
        _api = api;
        _visitService = visitService;
        _codeStorage = codeStorage;
        _fileSystemService = fileSystemService;
        _storage = bookingStorage;
        _visitMessageStorage = visitMessageStorage;
        _bookingDetailStorage = bookingDetailStorage;
        _visitStorage = bookingVisitStorage;
        _taskStorage = taskStorage;
        _visitTaskStorage = visitTaskStorage;
        _medicationStorage = medicationStorage;
        _visitMedicationStorage = visitMedicationStorage;
        _visitShortRemarkStorage = visitShortRemarkStorage;
        _visitHealthStatusStorage = visitHealthStatusStorage;
        _medicationTransactionStorage = medicationTransactionStorage;
        _visitFluidStorage = fluidStorage;
        _incidentStorage = incidentStorage;
        _bodyMapStorage = bodyMapStorage;
        _attachmentStorage = attachmentStorage;
        _careWorkerStorage = careWorkerStorage;
        _careWorkerService = careWorkerService;
        _serviceUserStorage = serviceUserStorage;
        _serviceUserService = serviceUserService;
        _serviceUserAddressStorage = serviceUserAddressStorage;
        _locationCentroidStorage = locationCentroidStorage;
        _visitConsumableStorage = bookingConsumableStorage;
    }

    public async Task<IList<Booking>> GetAllBookingsByDate(DateTime date)
    {
        return await _storage.GetAllByDate(date);
    }

    public async Task<IList<Booking>> GetBookingsOlderByDays(int days)
    {
        return await _storage.GetBookingsOlderByDays(days);
    }

    public async Task<Booking> GetCurrentBooking()
    {
        return await _storage.GetCurrentBooking();
    }

    public async Task<Booking> SetCurrentBookingStatus(int bookingId, ECodeType type, ECodeName name)
    {
        var booking = await GetById(bookingId);
        var code = await _codeStorage.GetByTypeValue(type, name);

        booking.CompletionStatusId = code.Id;

        if (name == ECodeName.COMPLETED || name == ECodeName.TAMPERED)
            booking.IsCompleted = true;

        return await InsertOrReplace(booking);
    }

    public async Task<Booking> GetNextBooking()
    {
        return await _storage.GetNextBooking();
    }

    public async Task<Booking> GetNextBooking(int prevBookingId)
    {
        return await _storage.GetNextBooking(prevBookingId);
    }

    public async Task<DateTime?> GetMinDateByRosterId(int rosterId)
    {
        return await _storage.GetMinDateByRosterId(rosterId);
    }

    public async Task DeleteCompleteBookings(IEnumerable<int> bookingIds)
    {
        var bookings = await _storage.GetAllByIds(bookingIds);
        var bookingDetails = await _bookingDetailStorage.GetAllByIds(bookingIds, "BookingId");
        var bookingDetailIds = bookingDetails.Select(x => x.Id).ToList();
        var medications = await _medicationStorage.GetAllByIds(bookingIds, "BookingId");
        var medicationIds = medications.Select(x => x.Id).ToList();
        var visits = await _visitStorage.GetAllByIds(bookingDetailIds, "BookingDetailId");
        var visitIds = visits.Select(x => x.Id).ToList();

        await _storage.DeleteAllByIds(bookingIds);
        await _bookingDetailStorage.DeleteAllByIds(bookingDetailIds);

        await _taskStorage.DeleteAllByIds(bookingIds, "BookingId");
        await _medicationTransactionStorage.DeleteAllByIds(medicationIds, "MedicationId");
        await _medicationStorage.DeleteAllByIds(bookingIds, "BookingId");

        await _visitService.DeleteVisitsByIds(visitIds);
    }

    public async Task<IList<Booking>> GetScheduledBookingsBetweenDates(DateTime StartDate, DateTime EndDate)
    {
        return await _storage.GetScheduledBookingsBetweenDates(StartDate, EndDate);
    }

    public async Task<IList<Booking>> GetPastBookings(int count)
    {
        return await _storage.GetPastBookings(count);
    }

    public async Task<BookingEditResponse> CheckBookingEditAccess(int bookingDetailId)
    {
        return await _api.CheckBookingEditAccess(bookingDetailId);
    }

    public async Task<bool?> CheckMasterCwChange(int bookingDetailId)
    {
        return await _api.CheckMasterCwChange(bookingDetailId);
    }

    public async Task<string> UpdateHandOverNotes(int bookingId)
    {
        var booking = await _storage.GetById(bookingId);
        booking.HandOverNotes = await _api.GetHandOverNotes(bookingId);
        await _storage.InsertOrReplace(booking);

        return booking.HandOverNotes;
    }

    public async Task<bool> DownloadRoster(int rosterId)
    {
        var bookings = await _api.DownloadRoster(rosterId);
        if (bookings == null)
            return false;

        await PersistBookings(bookings);
        return true;
    }

    public async Task<bool> SyncAllBookings()
    {
        var bookings = await _api.GetAllBookings();
        if (bookings == null)
            return false;

        await PersistBookings(bookings);
        return true;
    }

    public async Task DownloadVisitsByBookingId(int bookingId)
    {
        var bookingResponse = await _api.DownloadVisits(bookingId);
        await PersistBookings(bookingResponse);
    }

    public async Task<bool> SyncBookingById(int bookingId)
    {
        var booking = await _api.GetBooking(bookingId);
        if (booking == null)
            return false;

        await PersistBookings(booking);
        return true;
    }

    public async Task<IList<Booking>> GetAllBySuAndDate(int suId, DateTime date)
    {
        var bookings = await _api.GetAllBySuAndDate(suId, date);
        if (bookings == null)
            return null;

        await PersistBookings(bookings);
        return bookings.Bookings;
    }

    public async Task<IList<Booking>> GetAllByCwAndDate(int cwId, DateTime date)
    {
        var bookings = await _api.GetAllByCwAndDate(cwId, date);
        if (bookings == null)
            return null;

        await PersistBookings(bookings);
        return bookings.Bookings;
    }

    public async Task<List<Booking>> PersistBookings(BookingsResponse bookings)
    {
        if (bookings.VisitMessages != null && bookings.VisitMessages.Any())
            await _visitMessageStorage.InsertOrReplace(bookings.VisitMessages);

        if (bookings.Codes != null && bookings.Codes.Any())
            await _codeStorage.InsertOrReplace(bookings.Codes);

        if (bookings.Bookings != null && bookings.Bookings.Any())
            bookings.Bookings = (await InsertOrReplace(bookings.Bookings)).ToList();

        if (bookings.CareWorkers != null && bookings.CareWorkers.Any())
        {
            await _careWorkerService.DownloadProfilePictures(bookings.CareWorkers);
            await _careWorkerStorage.InsertOrReplace(bookings.CareWorkers);
        }

        if (bookings.ServiceUsers != null && bookings.ServiceUsers.Any())
        {
            await _serviceUserService.DownloadProfilePictures(bookings.ServiceUsers);
            await _serviceUserStorage.InsertOrReplace(bookings.ServiceUsers);
        }

        if (bookings.ServiceUsersAddresses != null && bookings.ServiceUsersAddresses.Any())
            await _serviceUserAddressStorage.InsertOrReplace(bookings.ServiceUsersAddresses);

        if (bookings.Centroids != null && bookings.Centroids.Any())
            await _locationCentroidStorage.InsertOrReplace(bookings.Centroids);

        if (bookings.BookingDetails != null && bookings.BookingDetails.Any())
            await _bookingDetailStorage.InsertOrReplace(bookings.BookingDetails);

        if (bookings.Tasks != null && bookings.Tasks.Any())
        {
            await _taskStorage.DeleteAllByIds(bookings.Bookings.Select(x => x.Id), "BookingId");
            await _taskStorage.InsertOrReplace(bookings.Tasks);
        }

        if (bookings.Medications != null && bookings.Medications.Any())
        {
            await _medicationStorage.DeleteAllByIds(bookings.Bookings.Select(x => x.Id), "BookingId");
            await _medicationStorage.InsertOrReplace(bookings.Medications);
        }

        if (bookings.Visits != null && bookings.Visits.Any())
        {
            await _visitStorage.InsertOrReplace(bookings.Visits);

            if (bookings.VisitTasks != null && bookings.VisitTasks.Any())
            {
                bookings.VisitTasks.ForEach(x => x.Id = x.ServerRef);
                await _visitTaskStorage.InsertOrReplace(bookings.VisitTasks);
            }

            if (bookings.VisitMedications != null && bookings.VisitMedications.Any())
            {
                bookings.VisitMedications.ForEach(x => x.Id = x.ServerRef);
                await _visitMedicationStorage.InsertOrReplace(bookings.VisitMedications);
            }

            if (bookings.Fluids != null && bookings.Fluids.Any())
            {
                bookings.Fluids.ForEach(x => x.Id = x.ServerRef);
                await _visitFluidStorage.InsertOrReplace(bookings.Fluids);
            }

            if (bookings.ShortRemarks != null && bookings.ShortRemarks.Any())
            {
                bookings.ShortRemarks.ForEach(x => x.Id = x.ServerRef);
                await _visitShortRemarkStorage.InsertOrReplace(bookings.ShortRemarks);
            }

            if (bookings.HealthStatuses != null && bookings.HealthStatuses.Any())
            {
                bookings.HealthStatuses.ForEach(x => x.Id = x.ServerRef);
                await _visitHealthStatusStorage.InsertOrReplace(bookings.HealthStatuses);
            }

            if (bookings.Incidents != null && bookings.Incidents.Any())
            {
                bookings.Incidents.ForEach(x => x.Id = x.ServerRef);
                await _incidentStorage.InsertOrReplace(bookings.Incidents);
            }

            if (bookings.BodyMaps != null && bookings.BodyMaps.Any())
            {
                bookings.BodyMaps.ForEach(x => x.Id = x.ServerRef);
                await _bodyMapStorage.InsertOrReplace(bookings.BodyMaps);
            }

            if (bookings.Attachments != null && bookings.Attachments.Any())
            {
                bookings.Attachments.ForEach(x => x.Id = x.ServerRef);
                bookings.Attachments.ToList().ForEach(i => i.FileName = Path.GetFileName(i.S3Url));
                await _attachmentStorage.InsertOrReplace(bookings.Attachments);
            }

            if (bookings.Consumables != null && bookings.Consumables.Any())
            {
                bookings.Consumables.ForEach(x => x.Id = x.ServerRef);
                await _visitConsumableStorage.InsertOrReplace(bookings.Consumables);
            }
        }

        return bookings.Bookings;
    }
}