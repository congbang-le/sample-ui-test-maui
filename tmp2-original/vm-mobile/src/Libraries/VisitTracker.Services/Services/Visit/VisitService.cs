namespace VisitTracker.Services;

public class VisitService : BaseService<Visit>, IVisitService
{
    private readonly IVisitStorage _storage;
    private readonly IVisitApi _api;
    private readonly ICodeStorage _codeStorage;
    private readonly IFileSystemService _fileSystemService;
    private readonly IBookingDetailStorage _bookingDetailStorage;
    private readonly IVisitTaskStorage _visitTaskStorage;
    private readonly IVisitMedicationStorage _visitMedicationStorage;
    private readonly IVisitFluidStorage _visitFluidStorage;
    private readonly IVisitShortRemarkStorage _visitShortRemarkStorage;
    private readonly IVisitHealthStatusStorage _visitHealthStatusStorage;
    private readonly IVisitConsumableStorage _visitConsumableStorage;
    private readonly IIncidentStorage _incidentStorage;
    private readonly IBodyMapStorage _bodyMapStorage;
    private readonly IAttachmentStorage _attachmentStorage;
    private readonly IStepCountStorage _stepCountStorage;
    private readonly ILocationStorage _locationStorage;

    public VisitService(IVisitStorage visitStorage,
        IVisitApi visitApi,
        ICodeStorage codeStorage,
        IFileSystemService fileSystemService,
        IBookingDetailStorage bookingDetailStorage,
        IVisitTaskStorage visitTaskStorage,
        IVisitMedicationStorage visitMedicationStorage,
        IVisitFluidStorage visitFluidStorage,
        IVisitShortRemarkStorage visitShortRemarkStorage,
        IVisitHealthStatusStorage visitHealthStatusStorage,
        IVisitConsumableStorage visitConsumableStorage,
        IIncidentStorage incidentStorage,
        IBodyMapStorage bodyMapStorage,
        IAttachmentStorage attachmentStorage,
        IStepCountStorage stepCountStorage,
        ILocationStorage locationStorage) : base(visitStorage)
    {
        _storage = visitStorage;
        _api = visitApi;
        _codeStorage = codeStorage;
        _fileSystemService = fileSystemService;
        _bookingDetailStorage = bookingDetailStorage;
        _visitTaskStorage = visitTaskStorage;
        _visitMedicationStorage = visitMedicationStorage;
        _visitFluidStorage = visitFluidStorage;
        _visitShortRemarkStorage = visitShortRemarkStorage;
        _visitHealthStatusStorage = visitHealthStatusStorage;
        _visitConsumableStorage = visitConsumableStorage;
        _incidentStorage = incidentStorage;
        _bodyMapStorage = bodyMapStorage;
        _attachmentStorage = attachmentStorage;
        _stepCountStorage = stepCountStorage;
        _locationStorage = locationStorage;
    }

    public async Task<Visit> GetByBookingDetailId(int bookingDetailId)
    {
        return await _storage.GetByBookingDetailId(bookingDetailId);
    }

    public async Task<IList<Visit>> GetCurrentWeekBookings()
    {
        // Get the current week's start and end dates
        DateTime currentDate = DateTimeExtensions.NowNoTimezone().Date;
        DateTime weekStartDate = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek);
        DateTime weekEndDate = weekStartDate.Date.AddDays(6);

        var allBookingVisits = await GetAll();
        return allBookingVisits.Where(x => x.CompletedOn <= weekEndDate && x.CompletedOn >= weekStartDate).ToList();
    }

    public async Task DeleteAllByBookingId(int bookingId)
    {
        await _storage.DeleteAllByBookingId(bookingId);
    }

    public async Task<Visit> GetByBookingId(int bookingId)
    {
        return await _storage.GetByBookingId(bookingId);
    }

    public async Task<Visit> GetByBookingIdForCurrentCw(int bookingId)
    {
        return await _storage.GetByBookingIdForCurrentCw(bookingId);
    }

    public async Task DeleteVisitsByBookingId(int bookingId)
    {
        var bookingDetails = await _bookingDetailStorage.GetAllForBooking(bookingId);
        if (bookingDetails != null && bookingDetails.Any())
        {
            var allVisits = await GetAllByIds(bookingDetails.Select(x => x.Id), "BookingDetailId");
            if (allVisits != null && allVisits.Any())
                await DeleteVisitsByIds(allVisits.Select(x => x.Id));
        }
    }

    public async Task DeleteVisitsByIds(IEnumerable<int> visitIds)
    {
        await _storage.DeleteAllByIds(visitIds);
        await _visitTaskStorage.DeleteAllByIds(visitIds, "VisitId");
        await _visitMedicationStorage.DeleteAllByIds(visitIds, "VisitId");
        await _visitFluidStorage.DeleteAllByIds(visitIds, "VisitId");
        await _visitHealthStatusStorage.DeleteAllByIds(visitIds, "VisitId");
        await _visitShortRemarkStorage.DeleteAllByIds(visitIds, "VisitId");
        await _visitConsumableStorage.DeleteAllByIds(visitIds, "VisitId");
        await _locationStorage.DeleteAllByIds(visitIds, "VisitId");
        await _stepCountStorage.DeleteAllByIds(visitIds, "VisitId");
        await _incidentStorage.DeleteAllByIds(visitIds, "VisitId");

        await _bodyMapStorage.DeleteAllByIds(visitIds, "BaseVisitId");
        var attachmentsToDelete = await _attachmentStorage.GetAllByIds(visitIds, "BaseVisitId");
        if (attachmentsToDelete != null && attachmentsToDelete.Any())
            await _fileSystemService.DeleteFiles(attachmentsToDelete);
        await _attachmentStorage.DeleteAllByIds(visitIds, "BaseVisitId");
    }

    public async Task<VisitDto> GetVisitDtoByBookingId(int bookingId)
    {
        var bookingDetails = await _bookingDetailStorage.GetAllForBooking(bookingId);
        var bookingDetailIds = bookingDetails.Select(x => x.Id).ToList();

        var visitDto = new VisitDto();
        visitDto.Visits = await _storage.GetAllByIds(bookingDetailIds, "BookingDetailId");
        var visitIds = visitDto.Visits.Select(x => x.Id).ToList();
        visitDto.VisitTasks = await _visitTaskStorage.GetAllByIds(visitIds, "VisitId");
        visitDto.VisitMedications = await _visitMedicationStorage.GetAllByIds(visitIds, "VisitId");
        visitDto.Fluids = await _visitFluidStorage.GetAllByIds(visitIds, "VisitId");
        visitDto.Incidents = await _incidentStorage.GetAllByIds(visitIds, "VisitId");
        visitDto.BodyMaps = await _bodyMapStorage.GetAllByIds(visitIds, "BaseVisitId");
        visitDto.Attachments = await _attachmentStorage.GetAllByIds(visitIds, "BaseVisitId");
        visitDto.Consumables = await _visitConsumableStorage.GetAllByIds(visitIds, "VisitId");
        visitDto.ShortRemarks = await _visitShortRemarkStorage.GetAllByIds(visitIds, "VisitId");
        visitDto.HealthStatuses = await _visitHealthStatusStorage.GetAllByIds(visitIds, "VisitId");

        return visitDto;
    }

    public async Task<string> CanStartVisit(StartVisitDto dto)
    {
        return await _api.CanStartVisit(dto);
    }

    public async Task<Visit> SyncVisit(Visit visit, VisitBiometricDto biometricDto = null)
    {
        var apiVisit = await _api.SyncVisit(visit, biometricDto);
        return await _storage.InsertOrReplace(apiVisit);
    }

    public async Task<bool> PingLocation(LastKnownDto dto)
    {
        return await _api.PingLocation(dto);
    }

    public async Task<bool> CanSubmitReport(int bookingId)
    {
        return await _api.CanSubmitReport(bookingId);
    }

    public async Task<bool> SubmitVisitReport(VisitReportDto dto)
    {
        return await _api.SubmitVisitReport(dto);
    }

    public async Task<bool> SubmitVisitEditReport(VisitReportEditDto dto)
    {
        return await _api.SubmitVisitEditReport(dto);
    }
}