namespace VisitTracker;

/// <summary>
/// DataRetentionService is a service that handles the removal of outdated data and incomplete booking edits in the application.
/// It uses the WeakReferenceMessenger to listen for messages related to data retention and performs the necessary actions when a message is received.
/// </summary>
public class DataRetentionService
{
    public DataRetentionService() => RegisterService();

    /// <summary>
    /// Removes all outdated data based on the user type.
    /// If the user type is SUPERVISOR, it retrieves all bookings. Otherwise, it retrieves bookings older than a specified number of days.
    /// </summary>
    /// <param name="userType"></param>
    /// <returns></returns>
    public async Task RemoveAllOutdated(string userType)
    {
        var bookings = userType == nameof(EUserType.SUPERVISOR) ? await AppServices.Current.BookingService.GetAll()
            : await AppServices.Current.BookingService.GetBookingsOlderByDays(Constants.NoOfDaysBookingsToShow);
        if (bookings != null && bookings.Any())
            await AppServices.Current.BookingService.DeleteCompleteBookings(bookings.Select(x => x.Id));

        await AppServices.Current.IncidentService.DeleteAdhocIncidents();
    }

    /// <summary>
    /// Removes incomplete booking edits by checking if the edit booking ID exists in preferences.
    /// If it does, it deletes the visits associated with that booking ID and removes the edit booking ID and visit content from preferences.
    /// </summary>
    /// <returns></returns>
    public async Task RemoveIncompleteBookingEdit()
    {
        if (Preferences.Default.ContainsKey(Constants.PrefKeyEditBookingId))
        {
            var bookingId = Preferences.Default.Get<int>(Constants.PrefKeyEditBookingId, default);
            if (bookingId == default) return;

            await AppServices.Current.VisitService.DeleteVisitsByBookingId(bookingId);
            var visitDtoStr = Preferences.Default.Get<string>(Constants.PrefKeyEditBookingVisitContent, null);
            var visitDto = JsonExtensions.Deserialize<VisitDto>(visitDtoStr);
            if (visitDto != null)
                await AppServices.Current.BookingService.PersistBookings(new BookingsResponse
                {
                    Visits = visitDto.Visits?.ToList(),
                    VisitTasks = visitDto.VisitTasks?.ToList(),
                    VisitMedications = visitDto.VisitMedications?.ToList(),
                    Fluids = visitDto.Fluids?.ToList(),
                    Incidents = visitDto.Incidents?.ToList(),
                    BodyMaps = visitDto.BodyMaps?.ToList(),
                    Attachments = visitDto.Attachments?.ToList(),
                    Consumables = visitDto.Consumables?.ToList(),
                    ShortRemarks = visitDto.ShortRemarks?.ToList(),
                    HealthStatuses = visitDto.HealthStatuses?.ToList()
                });
            else await AppServices.Current.BookingService.DownloadVisitsByBookingId(bookingId);

            Preferences.Default.Remove(Constants.PrefKeyEditBookingId);
            Preferences.Default.Remove(Constants.PrefKeyEditBookingVisitContent);
        }
    }

    /// <summary>
    /// Registers the DataRetentionService to listen for DataRetentionMessage events.
    /// When a message is received, it checks if the message value is true and calls the RemoveAllOutdated and RemoveIncompleteBookingEdit methods.
    /// </summary>
    public void RegisterService()
    {
        var isDataRetentionServiceRegistered = WeakReferenceMessenger.Default.IsRegistered<MessagingEvents.DataRetentionMessage>(this);
        if (!isDataRetentionServiceRegistered)
            WeakReferenceMessenger.Default.Register<MessagingEvents.DataRetentionMessage>(this,
                async (recipient, message) =>
                {
                    if (message.Value)
                    {
                        if (AppData.Current?.CurrentProfile?.Type != null)
                            await RemoveAllOutdated(AppData.Current?.CurrentProfile?.Type);
                        await RemoveIncompleteBookingEdit();
                    }
                });
    }
}