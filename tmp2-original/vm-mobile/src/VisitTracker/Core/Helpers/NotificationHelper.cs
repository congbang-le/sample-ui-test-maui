using System.Reactive.Threading.Tasks;

namespace VisitTracker;

/// <summary>
/// NotificationHelper is a singleton class that provides methods to handle push notifications in the application.
/// It includes methods to open notifications, handle notification actions, and manage notification-related data.
/// </summary>
public class NotificationHelper
{
    public static NotificationHelper Current => ServiceLocator.GetService<NotificationHelper>();

    public IList<ENotificationType> NonPersistentNotificationTypes =
    [
        ENotificationType.ESTIMATED_TIME_OF_ARRIVAL,
        ENotificationType.GPS_SIGNAL_LOST,
        ENotificationType.USER_REMOVED,
        ENotificationType.SU_PROFILE_UPDATED,
        ENotificationType.CW_PROFILE_UPDATED,
    ];

    public IList<ENotificationType> NonDuplicateNotificationTypes =
    [
        ENotificationType.ESTIMATED_TIME_OF_ARRIVAL,
        ENotificationType.GPS_SIGNAL_LOST,
    ];

    public IList<ENotificationType> BookingRelatedNotificationTypes =
    [
        ENotificationType.BOOKING_ADDED,
        ENotificationType.BOOKING_MISSED,
        ENotificationType.BOOKING_NON_MISSED,
        ENotificationType.BOOKING_CANCELLED,
        ENotificationType.BOOKING_UPDATE_BY_RESCHEDULED,
        ENotificationType.BOOKING_UPDATED,
        ENotificationType.BOOKING_REMOVED,
        ENotificationType.MASTER_CW_CHANGE,
        ENotificationType.VISIT_REPORT_UPLOADED,
        ENotificationType.BOOKING_ROSTER_CW,
        ENotificationType.BOOKING_ROSTER_SU
    ];

    public IList<ENotificationType> ProfileRelatedNotificationTypes =
    [
        ENotificationType.SU_PROFILE_UPDATED,
        ENotificationType.CW_PROFILE_UPDATED,
    ];

    /// <summary>
    /// Handles the action when a notification is opened by the user.
    /// It processes the notification based on its type and performs the necessary actions, such as displaying alerts or navigating to specific pages.
    /// </summary>
    /// <param name="notification">Notification object to be opened</param>
    /// <param name="userType">Type of the User</param>
    /// <returns></returns>
    public async Task OnNotificationOpened(Domain.Notification notification, string userType = "")
    {
        var notificationType = (ENotificationType)Enum.Parse(typeof(ENotificationType), notification.NotificationType);
        if (NonPersistentNotificationTypes.Contains(notificationType))
            return;

        switch (notificationType)
        {
            case ENotificationType.ADHOC_TEXT:
                await Application.Current.MainPage.DisplayAlert(Messages.Notification, notification.Content, Messages.Ok);
                break;

            case ENotificationType.ADHOC_HTML:
                var htmlPage = $"{nameof(HtmlPage)}?Id={notification.Id}";
                await Shell.Current.GoToAsync(htmlPage);
                break;

            case ENotificationType.BOOKING_REMOVED:
                var removedBooking = await AppServices.Current.BookingService.GetById(Convert.ToInt32(notification.TypeId));
                if (removedBooking != null) await AppServices.Current.BookingService.DeleteCompleteBookings([removedBooking.Id]);
                await Application.Current.MainPage.ShowSnackbar(Messages.BookingRemoved, false);
                break;

            case ENotificationType.BOOKING_ADDED:
            case ENotificationType.BOOKING_MISSED:
            case ENotificationType.BOOKING_NON_MISSED:
            case ENotificationType.BOOKING_CANCELLED:
            case ENotificationType.BOOKING_UPDATE_BY_RESCHEDULED:
            case ENotificationType.BOOKING_UPDATED:
                var booking = await AppServices.Current.BookingService.GetById(Convert.ToInt32(notification.TypeId));
                if (booking == null) await Application.Current.MainPage.ShowSnackbar(Messages.BookingNotRelevant, false);
                else await OpenActions.OpenBookingDetail(booking.Id);
                break;

            case ENotificationType.MASTER_CW_CHANGE:
                var bookingDetail = await AppServices.Current.BookingDetailService.GetById(Convert.ToInt32(notification.TypeId));
                await OpenActions.OpenBookingDetail(bookingDetail.BookingId);
                break;

            case ENotificationType.BOOKING_ROSTER_SU:
            case ENotificationType.BOOKING_ROSTER_CW:
                await OpenActions.OpenBookingsPage(notification.TypeId.Value);
                break;

            case ENotificationType.VISIT_REPORT_UPLOADED:
                await OpenActions.OpenBookingDetail(notification.TypeId.Value);
                break;

            case ENotificationType.EARLY_MEDICATION_RESPONSE_ADMIN:
            case ENotificationType.EARLY_MEDICATION_RESPONSE_NO_RESPONSE:
            case ENotificationType.EARLY_MEDICATION_RESPONSE_SKIP:
            case ENotificationType.LATE_MEDICATION_RESPONSE_ADMIN:
            case ENotificationType.LATE_MEDICATION_RESPONSE_SKIP:
            case ENotificationType.LATE_MEDICATION_RESPONSE_NO_RESPONSE:
                if (!notification.IsAcknowledged)
                    await OpenActions.OpenMedicationPage(notification.TypeId.Value);
                break;

            case ENotificationType.TP_NOTIFICATION:
                var url = SystemHelper.Current.GetUrl(Constants.TpUrlNotifications);
                await OpenActions.OpenMiscDetailPage(url, "Other Notifications");
                break;

            case ENotificationType.USER_REMOVED:
                await SystemHelper.Current.Logout();
                break;
        }

        var prevIsMarkedAsRead = notification.IsAcknowledged;
        if (!prevIsMarkedAsRead)
        {
            notification = await AppServices.Current.NotificationService.MarkAsRead(notification.Id);

            await AppServices.Current.SyncService.InsertOrReplace(
                new Sync
                {
                    Identifier = Constants.SyncNotificationAction,
                    Content = JsonExtensions.Serialize(new
                    {
                        notification.Id,
                        notification.AcknowledgedTime
                    })
                });
            await AppServices.Current.SyncService.SyncData();
            await AppServices.Current.SyncService.DeleteAllBySyncData();
        }

        await OnNotificationUnreadCountChanged();

        var tasks = new List<Task>();

        var notificationsInstance = ServiceLocator.GetService<NotificationsVm>();
        if (notificationsInstance != null) tasks.Add(notificationsInstance.InitCommand.Execute(true).ToTask());

        if (userType == nameof(EUserType.CAREWORKER))
        {
            var careDashboardVm = ServiceLocator.GetService<CareWorkerDashboardVm>();
            if (careDashboardVm != null) tasks.Add(careDashboardVm.InitCommand.Execute(true).ToTask());
        }
        else if (userType == nameof(EUserType.SERVICEUSER))
        {
            var suDashboardVm = ServiceLocator.GetService<ServiceUserDashboardVm>();
            if (suDashboardVm != null) tasks.Add(suDashboardVm.InitCommand.Execute(true).ToTask());
        }
        else if (userType == "")
        {
            var profileType = AppData.Current.CurrentProfile.Type;
            if (profileType == nameof(EUserType.NEXTOFKIN) || profileType == nameof(EUserType.SERVICEUSER))
            {
                var suDashboardVm = ServiceLocator.GetService<ServiceUserDashboardVm>();
                if (suDashboardVm != null) tasks.Add(suDashboardVm.InitCommand.Execute(true).ToTask());
            }
            else
            {
                var careDashboardVm = ServiceLocator.GetService<CareWorkerDashboardVm>();
                if (careDashboardVm != null) tasks.Add(careDashboardVm.InitCommand.Execute(true).ToTask());
            }
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Handles the action when a notification is received.
    /// It processes the notification based on its type and performs the necessary actions, such as updating booking status or downloading rosters.
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    public async Task OnNotificationReceived(Domain.Notification notification)
    {
        var notificationType = (ENotificationType)Enum.Parse(typeof(ENotificationType), notification.NotificationType);
        switch (notificationType)
        {
            case ENotificationType.BOOKING_ROSTER_SU:
            case ENotificationType.BOOKING_ROSTER_CW:
                await AppServices.Current.BookingService.DownloadRoster(notification.TypeId.Value);
                break;

            case ENotificationType.BOOKING_CANCELLED:
                var cancelledCode = await AppServices.Current.CodeService.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.CANCELLED);
                var cancelledBooking = await AppServices.Current.BookingService.GetById(notification.TypeId.Value);
                cancelledBooking.BookingStatusId = cancelledCode.Id;
                await AppServices.Current.BookingService.InsertOrReplace(cancelledBooking);
                break;

            case ENotificationType.BOOKING_MISSED:
                var missedCode = await AppServices.Current.CodeService.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.MISSED);
                var missedBooking = await AppServices.Current.BookingService.GetById(notification.TypeId.Value);
                missedBooking.BookingStatusId = missedCode.Id;
                await AppServices.Current.BookingService.InsertOrReplace(missedBooking);
                break;

            case ENotificationType.BOOKING_NON_MISSED:
                var scheduledCode = await AppServices.Current.CodeService.GetByTypeValue(ECodeType.BOOKING_STATUS, ECodeName.SCHEDULED);
                var scheduledBooking = await AppServices.Current.BookingService.GetById(notification.TypeId.Value);
                scheduledBooking.BookingStatusId = scheduledCode.Id;
                await AppServices.Current.BookingService.InsertOrReplace(scheduledBooking);
                break;

            case ENotificationType.BOOKING_ADDED:
                await AppServices.Current.BookingService.SyncBookingById(notification.TypeId.Value);
                break;

            case ENotificationType.BOOKING_REMOVED:
                await AppServices.Current.BookingService.DeleteCompleteBookings([notification.TypeId.Value]);
                break;

            case ENotificationType.BOOKING_UPDATED:
                await AppServices.Current.BookingService.DeleteCompleteBookings([notification.TypeId.Value]);
                await AppServices.Current.BookingService.SyncBookingById(notification.TypeId.Value);
                break;

            case ENotificationType.BOOKING_UPDATE_BY_RESCHEDULED:
                var exBookingId = Convert.ToInt32(notification.Data);
                await AppServices.Current.BookingService.DeleteCompleteBookings([exBookingId]);
                await AppServices.Current.BookingService.SyncBookingById(exBookingId);
                await AppServices.Current.BookingService.SyncBookingById(notification.TypeId.Value);
                break;

            case ENotificationType.VISIT_REPORT_UPLOADED:
                if (!Preferences.Default.ContainsKey(Constants.PrefKeyOngoingNonMasterCwBookingId))
                {
                    await AppServices.Current.VisitService.DeleteVisitsByBookingId(notification.TypeId.Value);
                    await AppServices.Current.BookingService.DownloadVisitsByBookingId(notification.TypeId.Value);
                }
                break;

            case ENotificationType.EARLY_MEDICATION_RESPONSE_NO_RESPONSE:
            case ENotificationType.LATE_MEDICATION_RESPONSE_NO_RESPONSE:
                var medicationNoResponse = await AppServices.Current.VisitMedicationService.GetByMedicationId(notification.TypeId.Value);
                medicationNoResponse.CanAdmisterMedication = true;
                medicationNoResponse.HasNoResponse = true;
                await AppServices.Current.VisitMedicationService.InsertOrReplace(medicationNoResponse);
                WeakReferenceMessenger.Default.Send(new MessagingEvents.MedicationPageUpdateReceivedMessage(true));
                break;
            case ENotificationType.EARLY_MEDICATION_RESPONSE_ADMIN:
            case ENotificationType.LATE_MEDICATION_RESPONSE_ADMIN:
                var medicationAdminister = await AppServices.Current.VisitMedicationService.GetByMedicationId(notification.TypeId.Value);
                medicationAdminister.CanAdmisterMedication = true;
                await AppServices.Current.VisitMedicationService.InsertOrReplace(medicationAdminister);
                WeakReferenceMessenger.Default.Send(new MessagingEvents.MedicationPageUpdateReceivedMessage(true));
                break;

            case ENotificationType.EARLY_MEDICATION_RESPONSE_SKIP:
            case ENotificationType.LATE_MEDICATION_RESPONSE_SKIP:
                var medicationSkip = await AppServices.Current.VisitMedicationService.GetByMedicationId(notification.TypeId.Value);
                medicationSkip.CanAdmisterMedication = false;
                medicationSkip.CompletionStatusId = AppData.Current.Codes.FirstOrDefault(x =>
                    x.Type == ECodeType.MEDICATION_COMPLETION.ToString() &&
                    x.Name == ECodeName.UNABLE_TO_COMPLETE.ToString()).Id;
                medicationSkip.CompletionStatusDetailId = AppData.Current.Codes.FirstOrDefault(x =>
                    x.Type == ECodeType.MEDICATION_UNABLE_TO_COMPLETE.ToString() &&
                    x.Name == ECodeName.NOT_APPROVED.ToString()).Id;
                await AppServices.Current.VisitMedicationService.InsertOrReplace(medicationSkip);
                WeakReferenceMessenger.Default.Send(new MessagingEvents.MedicationPageUpdateReceivedMessage(true));
                break;

            case ENotificationType.MASTER_CW_CHANGE:
                var bookingDetail = await AppServices.Current.BookingDetailService.GetById(notification.TypeId.Value);
                var dbBookingDetails = await AppServices.Current.BookingDetailService.GetAllForBooking(bookingDetail.BookingId);
                foreach (var dbBookingDetail in dbBookingDetails)
                    dbBookingDetail.IsMaster = dbBookingDetail.Id == bookingDetail.Id;
                await AppServices.Current.BookingDetailService.InsertOrReplace(dbBookingDetails);
                break;

            case ENotificationType.ESTIMATED_TIME_OF_ARRIVAL:
                var etaList = JsonExtensions.Deserialize<List<EtaMobileDto>>(notification.Data);
                foreach (var eta in etaList)
                {
                    var bd = await AppServices.Current.BookingDetailService.GetById(eta.BookingDetailId);
                    bd.Eta = eta.Eta;
                    bd.EtaAvailable = eta.EtaAvailable;
                    bd.EtaOn = eta.EtaOn;
                    bd.EtaStatusColor = eta.EtaStatusColor;
                    bd.EtaStatusText = eta.EtaStatusText;
                    await AppServices.Current.BookingDetailService.InsertOrReplace(bd);
                }
                WeakReferenceMessenger.Default.Send(new MessagingEvents.PreVisitMonitorMessage(true));
                break;

            case ENotificationType.TP_NOTIFICATION:
                break;

            case ENotificationType.USER_REMOVED:
                await SystemHelper.Current.Logout();
                break;

            case ENotificationType.SU_PROFILE_UPDATED:
                await AppServices.Current.ServiceUserService.SyncServiceUser(notification.TypeId.Value);
                break;

            case ENotificationType.CW_PROFILE_UPDATED:
                await AppServices.Current.CareWorkerService.SyncCareWorker(notification.TypeId.Value);
                break;
        }

        if (!NotificationHelper.Current.NonPersistentNotificationTypes.Contains(
                (ENotificationType)System.Enum.Parse(typeof(ENotificationType), notification.NotificationType)))
        {
            notification.IsSynced = true;
            notification = await AppServices.Current.NotificationService.InsertOrReplace(notification);
            await OnNotificationUnreadCountChanged();
        }

        if (BookingRelatedNotificationTypes.Contains(notificationType)
            || ProfileRelatedNotificationTypes.Contains(notificationType))
        {
            if (AppData.Current.CurrentProfile.Type == nameof(EUserType.CAREWORKER))
            {
                var cwDashboardVm = ServiceLocator.GetService<CareWorkerDashboardVm>();
                if (cwDashboardVm != null) await cwDashboardVm.InitCommand.Execute(true);

                var ongoingVm = ServiceLocator.GetService<OngoingVm>();
                var doRefreshOngoing = false;
                if (ongoingVm?.OngoingDto?.Booking == null || ongoingVm.ShowStartVisitButton)
                    doRefreshOngoing = true;

                if (ongoingVm.OngoingDto != null && BookingRelatedNotificationTypes.Contains(notificationType)
                         && ((notificationType != ENotificationType.MASTER_CW_CHANGE && notification.TypeId.Value == ongoingVm.OngoingDto.Booking.Id)
                            || (notificationType == ENotificationType.MASTER_CW_CHANGE && ongoingVm.OngoingDto.BookingDetails.Any(x => x.Id == notification.TypeId.Value))))
                {
                    Preferences.Default.Remove(Constants.PrefKeyOngoingNonMasterCwBookingId);
                    doRefreshOngoing = true;
                }

                if ((notificationType == ENotificationType.BOOKING_CANCELLED || notificationType == ENotificationType.BOOKING_REMOVED)
                        && AppServices.Current.CareWorkerTrackerService.OnGoingBooking?.Id == notification.TypeId.Value)
                {
                    Preferences.Default.Remove(Constants.PrefKeyOngoingNonMasterCwBookingId);
                    AppServices.Current.CareWorkerTrackerService.StopPersistence = true;
                    await AppServices.Current.CareWorkerTrackerService.StopNormalMode(ESensorExitReason.BOOKING_CANCELLED);
                    if (AppServices.Current.CareWorkerTrackerService.NextOrUpcomingBookingDetailId.HasValue)
                        await AppServices.Current.CareWorkerTrackerService.StartNormalModeByAck(AppServices.Current.CareWorkerTrackerService.NextOrUpcomingBookingDetailId.Value);
                    doRefreshOngoing = true;
                }

                if (doRefreshOngoing)
                    await ongoingVm.InitCommand.Execute(true);
            }
            else if (AppData.Current.CurrentProfile.Type == nameof(EUserType.SUPERVISOR))
            {
                var supDashboardVm = ServiceLocator.GetService<SupervisorDashboardVm>();
                if (supDashboardVm != null) await supDashboardVm.InitCommand.Execute(true);
            }
            else if (AppData.Current.CurrentProfile.Type == nameof(EUserType.NEXTOFKIN) || AppData.Current.CurrentProfile.Type == nameof(EUserType.SERVICEUSER))
            {
                var suDashboardVm = ServiceLocator.GetService<ServiceUserDashboardVm>();
                if (suDashboardVm != null) await suDashboardVm.InitCommand.Execute(true);
            }

            if (AppData.Current.CurrentProfile.Type != nameof(EUserType.SUPERVISOR))
            {
                var bookingsVm = ServiceLocator.GetService<BookingsVm>();
                if (bookingsVm != null) await bookingsVm.InitCommand.Execute(true);
            }
        }

        var notificationsVm = ServiceLocator.GetService<NotificationsVm>();
        if (notificationsVm != null) await notificationsVm.InitCommand.Execute(true);
    }

    /// <summary>
    /// Handles the action when the unread count of notifications changes.
    /// It updates the badge count on the notification tab and sets the app badge count.
    /// </summary>
    /// <param name="notificationsUnreadCount"></param>
    /// <returns></returns>
    public async Task OnNotificationUnreadCountChanged(int? notificationsUnreadCount = null)
    {
        if (AppData.Current.CurrentProfile.Type == nameof(EUserType.SUPERVISOR))
            return;

        if (notificationsUnreadCount == null)
        {
            var notificationsUnread = await AppServices.Current.NotificationService.GetUnreadNotifications();
            notificationsUnreadCount = notificationsUnread.Count;
        }

        var notificationTabIndex = await App.GetTabIndexByVm(nameof(NotificationsVm));
        WeakReferenceMessenger.Default.Send(new MessagingEvents.TabBadgeCountMessage(
            (notificationTabIndex, notificationsUnreadCount.Value)));
        AppServices.Current.AppBadge.SetCount((uint)notificationsUnreadCount.Value);
    }

    /// <summary>
    /// Synchronizes notifications from the last ID.
    /// It retrieves all notifications and checks for any missed notifications based on the maximum ID.
    /// </summary>
    /// <returns></returns>
    public async Task SyncNotificationFromLast()
    {
        var notifications = await AppServices.Current.NotificationService.GetAll();
        if (notifications != null && notifications.Any())
        {
            var missedNotifications = await AppServices.Current.NotificationService.SyncByLastId(notifications.MaxBy(x => x.Id).Id);
            if (missedNotifications != null && missedNotifications.Any())
            {
                _ = Task.Run(async () =>
                {
                    foreach (var notification in missedNotifications.OrderBy(x => x.CreatedTime))
                        await NotificationHelper.Current.OnNotificationReceived(notification);
                });
            }
        }
    }
}