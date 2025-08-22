namespace VisitTracker;

public class BaseDto : ReactiveObject
{
}

public class LoginProviderDto : BaseDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string ImageUrl { get; set; }
}

public class ErrorDto : BaseDto
{
    public EFeaturePermissionOrderedType Type { get; set; }
    public string Error { get; set; }
    public ReactiveCommand<EFeaturePermissionOrderedType, Unit> ExecuteCommand { get; set; }
    public string TypeAsString => Type.ToString();
}

public class CodeDto : BaseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Order { get; set; }
}

public class VisitMessageDto : BaseDto
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
}

public class BookingBaseDto : BaseDto
{
    public int? Id { get; set; }
    public DateTime? CompletedOn { get; set; }
    public short? Order { get; set; }
    public Color CompletionStatusColor { get; set; }
    public bool IsDrafted { get; set; } = false;
}

public class BookingCardDto : BaseDto
{
    public Booking Booking { get; set; }
    public ServiceUser ServiceUser { get; set; }
    public CareWorker PrimaryCareWorker { get; set; }
    public EtaMobileDto PrimaryCareWorkerEta { get; set; }
    public CareWorker SecondaryCareWorker { get; set; }
    public EtaMobileDto SecondaryCareWorkerEta { get; set; }
    public IList<Visit> Visits { get; set; }
}

public class BookingsDto : BaseDto
{
    public int Id { get; set; }

    public string ServiceUserImageUrl { get; set; }

    public string ServiceUserName { get; set; }

    public string ServiceUserAddress { get; set; }

    public string CareWorkerImageUrl { get; set; }

    public string CareWorkerName { get; set; }

    public string CareWorkerAddress { get; set; }

    public string BookingFromTime { get; set; }

    public string BookingFromToTime { get; set; }

    public bool IsCompleted { get; set; }

    public IList<BookingCareWorkerDto> CareWorkers { get; set; }

    public IList<Visit> Visits { get; set; }
}

public class ServiceUserDto : BaseDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public int GenderId { get; set; }
    public string Gender { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}

public class TaskDto : BookingBaseDto
{
    public string Title { get; set; }
    public bool IsVisited { get; set; }
}

public class MedicationDto : BookingBaseDto
{
    public string Name { get; set; }
    public bool AdminsterWarning { get; set; } = false;
}

public class FluidDto : BookingBaseDto
{
    public string Title { get; set; }
    public bool IsVisited { get; set; }
}

public class BodyMapDto : BookingBaseDto
{
    public string Notes { get; set; }
    public string DisplayName { get; set; }
}

public class IncidentsDto : BookingBaseDto
{
    public string Summary { get; set; }
    public int BookingId { get; set; }
}

public class AttachmentDto : BaseDto
{
    public int Id { get; set; }
    public int ServerRef { get; set; }
    public bool IsPlaying { get; set; } = false;
    public string DisplayIcon { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string S3Url { get; set; }
    public EAttachmentType AttachmentType { get; set; }
}

public class VisitConsumableDto : BaseDto
{
    public int ConsumableTypeId { get; set; }
    public string ConsumableTypeStr { get; set; }
    public int? QuantityUsed { get; set; }
}

public class BodyMapMaintainDto : BaseDto
{
    public string Notes { get; set; }
    public List<int> SelectedBodyParts { get; set; }
}

public class BodyMapCreateDto : BaseDto
{
    public string BodyMapLabel { get; set; } = Constants.BodyMapBack;
    public bool NotesEnabled { get; set; } = false;
    public bool IsChangeBodyMap => BodyMapLabel == Constants.BodyMapFront;
}

public class BookingDetailDto : BaseDto
{
    public int Id { get; set; }
    public Visit Visit { get; set; }
    public UserCardDto ServiceUserCard { get; set; }
    public string BookingStatus { get; set; }
    public string HandOverNotes { get; set; }
    public string Notes { get; set; }
    public string Summary { get; set; }
    public ServiceUserDto ServiceUser { get; set; }

    public bool IsFluidApplicable { get; set; }

    public List<TaskDto> TaskList { get; set; }
    public List<MedicationDto> MedicationList { get; set; }
    public FluidDto Fluid { get; set; }
    public IList<BodyMap> BodyMapList { get; set; }
    public IList<Incident> IncidentList { get; set; }
    public IList<AttachmentDto> AttachmentList { get; set; }

    public string SelectedConsumables { get; set; }
    public string SelectedHealthStatus { get; set; }
    public string SelectedShortRemark { get; set; }
    public string BookingStatusColor { get; set; }
    public bool IsVisited { get; set; }
    public string PageTitle { get; set; }
}

public class MissingFingerprintDto : BaseDto
{
    public ServiceUser ServiceUser { get; set; }
    public string IsAndroidMissing { get; set; }
    public string IsiOSMissing { get; set; }
}

public class SupervisorDashboardDto : BaseDto
{
    public IList<MissingFingerprintDto> MissingFingerprints { get; set; }
    public IList<ServiceUser> MissingGroundTruths { get; set; }

    public IList<Domain.Notification> NotificationPendingAck { get; set; }
    public string PendingNotificationTitle { get; set; }
    public string PendingNotificationCount { get; set; }

    public int PendingFormsCount { get; set; }
    public int ScheduledFormsCount { get; set; }
    public int SubmittedFormsCount { get; set; }
    public int OverdueFormsCount { get; set; }

    public string ProfileName { get; set; }

    public double CardViewHeight { get; set; }
}

public class ServiceUserDashboardDto : BaseDto
{
    public UserCardDto CareWorkerCard { get; set; }
    public int? UpcomingCareWorkerId { get; set; }
    public List<BookingCardDto> Bookings { get; set; }
    public string PendingNotificationTitle { get; set; }
    public string ETAStatus { get; set; }
    public string TotalBookingTitle { get; set; }
    public string PendingNotificationCount { get; set; }
    public TotalBookingCountsDto TotalBookings { get; set; } = new TotalBookingCountsDto();
    public TotalBookingCountsDto CompletedBookings { get; set; } = new TotalBookingCountsDto();
    public string ProfileName { get; set; }
    public double UpcomingBookingsHeight { get; set; }
    public bool IsVisibleNoData { get; set; }
}

public class CareWorkerDashboardDto : BaseDto
{
    public UserCardDto ServiceUserCard { get; set; }
    public IList<BookingCareWorkerDto> CareWorkers { get; set; }
    public List<BookingCardDto> Bookings { get; set; }
    public string ETAStatus { get; set; }
    public TotalBookingCountsDto TotalBookings { get; set; } = new TotalBookingCountsDto();
    public TotalBookingCountsDto CompletedBookings { get; set; } = new TotalBookingCountsDto();
    public string UpcomingTravelTime { get; set; }
    public string TotalBookingTitle { get; set; }
    public string PendingNotificationTitle { get; set; }
    public string ProfileName { get; set; }
    public string PendingNotificationCount { get; set; }
    public string ArrivalTimeLebel { get; set; }
    public double UpcomingBookingsHeight { get; set; }
    public bool IsVisibleNoData { get; set; }
}

public class TotalBookingCountsDto : BaseDto
{
    public int Mon { get; set; }
    public int Tue { get; set; }
    public int Wed { get; set; }
    public int Thu { get; set; }
    public int Fri { get; set; }
    public int Sat { get; set; }
    public int Sun { get; set; }

    public Color MonColor { get; set; }
    public Color TueColor { get; set; }
    public Color WedColor { get; set; }
    public Color ThuColor { get; set; }
    public Color FriColor { get; set; }
    public Color SatColor { get; set; }
    public Color SunColor { get; set; }
}

public class FluidChartDto : BaseDto
{
    public List<FluidHistoryDto> FluidsList { get; set; } = new List<FluidHistoryDto>();

    public int? OralIntakeTotal { get; set; }
    public int? IvScIntakeTotal { get; set; }
    public int? OtherIntakeTotal { get; set; }
    public int? UrineOutputTotal { get; set; }
    public int? VomitOutputTotal { get; set; }
    public int? TubeOutputTotal { get; set; }
    public int? OtherOutputTotal { get; set; }
    public int? TodayBalance { get; set; }
    public int? YesterdayBalance { get; set; }
    public string TodayBalanceTime { get; set; }

    public Color TodayBalanceColor => TodayBalance.HasValue && TodayBalance.Value <= 0 ? Colors.Red : Colors.Green;
    public Color YesterdayBalanceColor => YesterdayBalance.HasValue && YesterdayBalance.Value <= 0 ? Colors.Red : Colors.Green;
}

public class FluidHistoryDto : BaseDto
{
    public int? OralIntake { get; set; }
    public int? IvScIntake { get; set; }
    public int? OtherIntake { get; set; }
    public int? UrineOutput { get; set; }
    public int? VomitOutput { get; set; }
    public int? TubeOutput { get; set; }
    public int? OtherOutput { get; set; }
    public string Hour { get; set; }
}

public class FluidDetailDto : BaseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public CodeDto CompletionStatus { get; set; }
    public DateTime? CompletedOn { get; set; }
    public string BookingFromToTime { get; set; }
    public string BookingTypeWithCode { get; set; }
    public int? OralIntake { get; set; }
    public int? IvScIntake { get; set; }
    public int? OtherIntake { get; set; }
    public int? UrineOutput { get; set; }
    public int? VomitOutput { get; set; }
    public int? TubeOutput { get; set; }
    public int? OtherOutput { get; set; }
    public ServiceUserDto ServiceUser { get; set; }
    public IList<AttachmentDto> AttachmentList { get; set; }

    public List<BodyMapDto> BodyMapList { get; set; } = new List<BodyMapDto>();
}

public class IncidentDetailDto : BaseDto
{
    public int Id { get; set; }
    public string BookingFromToTime { get; set; }
    public string BookingTypeWithCode { get; set; }
    public bool AtSuLocation { get; set; }
    public bool DuringOnGoingVisit { get; set; }
    public string Location { get; set; }
    public DateTime IncidentDate { get; set; } = DateTimeExtensions.NowNoTimezone();
    public TimeSpan IncidentTime { get; set; } = DateTimeExtensions.NowNoTimezone().TimeOfDay;
    public string Summary { get; set; }
    public ServiceUserDto ServiceUser { get; set; }
    public CodeDto Type { get; set; }
    public CodeDto Injury { get; set; }
    public CodeDto Treatment { get; set; }
    public string OtherType { get; set; }
    public string OtherInjury { get; set; }
    public string CWImageUrl { get; set; }
    public string CwName { get; set; }
    public List<BodyMapDto> BodyMapList { get; set; }
    public IList<AttachmentDto> AttachmentList { get; set; }
}

public class MarChartDto : BaseDto
{
    public string BookingFromToTime { get; set; }
    public string BookingTypeWithCode { get; set; }
    public ServiceUser ServiceUser { get; set; }
    public ObservableCollection<MedicationsDto> Medications { get; set; }
}

public class MedicationsDto : BaseDto
{
    public string MedicationName { get; set; }
    public string Strength { get; set; }
    public string GracePriod { get; set; }
    public string Dosage { get; set; }
    public string Mode { get; set; }
    public string Route { get; set; }
    public string MealInstructions { get; set; }
    public ObservableCollection<MedicationTimeAndDetailDto> MedicationTimeAndDetailList { get; set; }
}

public class MedicationTimeAndDetailDto : BaseDto
{
    public string Time { get; set; }
    public MedicationDetailHistoryDto PrevSelectedMedication { get; set; }
    public MedicationDetailHistoryDto SelectedMedication { get; set; }
    public ObservableCollection<MedicationDetailHistoryDto> MedicationDetails { get; set; }

    public MedicationHistoryDto SelectedRecord { get; set; }

    public bool IsBusy { get; set; } = false;
}

public class MedicationDetailHistoryDto : BaseDto
{
    public int Id { get; set; }
    public string Month { get; set; }
    public string Date { get; set; }
    public string Day { get; set; }
    public Color BackgroundColor { get; set; }
    public bool IsSelected { get; set; } = false;
}

public class MedicationHistoryDto : BaseDto
{
    public bool IsAvailable { get; set; } = true;
    public DateTime Time { get; set; }
    public string Completion { get; set; }
    public string Summary { get; set; }
    public string CompletionDetail { get; set; }
    public Color BackgroundColor { get; set; }
}

public class ExternalLinkDto : BaseDto
{
    public EExternalLinkType LinkType { get; set; }
    public string Title { get; set; }
    public string ServerUrl { get; set; }
    public string Icon { get; set; }
}

public class NotificationsDto : BaseDto
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string Type { get; set; }
    public int? TypeId { get; set; }
    public string Description { get; set; }

    public string Icon { get; set; }

    public Color Color { get; set; }

    public bool IsAcknowledged { get; set; }
    public bool RequireAcknowledgement { get; set; }

    public DateTime? CreatedTime { get; set; }
}

public class OngoingDto : BaseDto
{
    public Booking Booking { get; set; }
    public Visit Visit { get; set; }
    public UserCardDto ServiceUserCard { get; set; }
    public BookingDetail BookingDetail { get; set; }
    public IList<BookingDetail> BookingDetails { get; set; }
    public bool IsFluidApplicable { get; set; }
    public bool IsMaster { get; set; }

    public string BookingFromToTime { get; set; }
    public string BookingTypeWithCode { get; set; }
    public string BookingStatus { get; set; }
    public string Notes { get; set; }
    public string HandOverNotes { get; set; }
    public string HandOverNotesEntered { get; set; }
    public string Summary { get; set; }

    public ServiceUser ServiceUser { get; set; }
    public IList<BookingCareWorkerDto> CareWorkers { get; set; }

    public FluidDto Fluid { get; set; }
    public IList<TaskDto> TaskList { get; set; }
    public IList<MedicationDto> MedicationList { get; set; }
    public IList<BodyMap> BodyMapList { get; set; }
    public IList<Incident> IncidentList { get; set; }
    public IList<VisitConsumableDto> ConsumableList { get; set; }

    [Reactive] public ObservableCollection<string> SelectedHealthStatuses { get; set; }
    [Reactive] public ObservableCollection<string> HealthStatuses { get; set; }

    [Reactive] public ObservableCollection<string> SelectedShortRemarks { get; set; }
    [Reactive] public ObservableCollection<string> ShortRemarks { get; set; }

    public IList<AttachmentDto> AttachmentList { get; set; }
    public string MaxCharRemainingText { get; set; }
    public string ETA { get; set; }
    public string ArrivalStatus { get; set; }
    public string ArrivalStatusColor { get; set; }
}

public class UserCardDto : BaseDto
{
    public int UserId { get; set; }
    public EUserType UserType { get; set; }

    public string Name { get; set; }
    public string Phone { get; set; }
    public string ImageUrl { get; set; }
    public string Gender { get; set; }

    public int GenderId { get; set; }
    public string TransportationMode { get; set; }

    public UserBookingCardDto UserBookingCard { get; set; }
    public UserAddressCardDto UserAddressCard { get; set; }
}

public class UserBookingCardDto : BaseDto
{
    public string BookingFromToTime { get; set; }
    public string BookingTypeWithCode { get; set; }
}

public class UserAddressCardDto : BaseDto
{
    public string Address { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string KeySafePIN { get; set; }
    public string EntryInstructions { get; set; }
    public List<AttachmentDto> HousePhotos { get; set; }
}

public class EtaMobileDto : BaseDto
{
    public int BookingDetailId { get; set; }
    public string Eta { get; set; }
    public string EtaOn { get; set; }
    public string EtaStatusColor { get; set; }
    public string EtaStatusText { get; set; }
    public bool EtaAvailable { get; set; }
}

public class BookingCareWorkerDto : BaseDto
{
    public int BookingDetailId { get; set; }
    public string ImageUrl { get; set; }
    public string Name { get; set; }
    public bool IsMaster { get; set; }

    public string Eta { get; set; }
    public string EtaOn { get; set; }
    public string EtaStatusColor { get; set; }
    public string EtaStatusText { get; set; }
    public bool EtaAvailable { get; set; }
}