using System.Text.Json;

namespace VisitTracker;

public static class OpenActions
{
   public static async Task OpenMiscDetailPage(string url, string title = "", Dictionary<string, string> additionalParams = null)
    {
        var miscDetailPage = $"{nameof(MiscellaneousDetailPage)}?ServerUrl={url}&Title={title}";
        if (additionalParams != null && additionalParams.Any())
        {
            string jsonString = JsonSerializer.Serialize(additionalParams);
            miscDetailPage = miscDetailPage + $"&AdditionalParams={Uri.EscapeDataString(jsonString)}";
        }
        await Shell.Current.GoToAsync(miscDetailPage);
    }

    public static async Task OpenBookingDetail(int bookingId)
    {
        var ongoingVm = ServiceLocator.GetService<OngoingVm>();
        if (ongoingVm?.OngoingDto?.Booking != null && ongoingVm.OngoingDto.Booking.Id == bookingId)
            ongoingVm.RefreshOnAppear = true;

        var bookingsVm = ServiceLocator.GetService<BookingsVm>();
        if (bookingsVm != null)
            bookingsVm.RefreshOnAppear = true;

        var bookingDetailPage = $"{nameof(BookingDetailPage)}?Id={bookingId}";
        await Shell.Current.GoToAsync(bookingDetailPage);
    }

    public static async Task OpenServiceUserDetail(int serviceUserId)
    {
        var supervisorDashboardVm = ServiceLocator.GetService<SupervisorDashboardVm>();
        if (supervisorDashboardVm != null)
            supervisorDashboardVm.RefreshOnAppear = true;

        var serviceUsersVm = ServiceLocator.GetService<ServiceUsersVm>();
        if (serviceUsersVm != null)
            serviceUsersVm.RefreshOnAppear = true;

        var bookingDetailPage = $"{nameof(ServiceUserDetailPage)}?Id={serviceUserId}";
        await Shell.Current.GoToAsync(bookingDetailPage);
    }

    public static async Task OpenBookingsPage(int? rosterId = null)
    {
        var bookingsVm = ServiceLocator.GetService<BookingsVm>();
        bookingsVm.RosterId = rosterId;
        await bookingsVm.InitCommand.Execute(true);

        var bookingsTabIndex = await App.GetTabIndexByVm(nameof(BookingsVm));
        Shell.Current.CurrentItem = Shell.Current.Items[0].Items[bookingsTabIndex];
    }

    public static async Task OpenOnGoingPage()
    {
        var ongoingTabIndex = await App.GetTabIndexByVm(nameof(OngoingVm));
        Shell.Current.CurrentItem = Shell.Current.Items[0].Items[ongoingTabIndex];
    }

    public static async Task OpenNotificationsPage()
    {
        var notificationVm = ServiceLocator.GetService<NotificationsVm>();
        await notificationVm.InitCommand.Execute(true);

        var notificationTabIndex = await App.GetTabIndexByVm(nameof(NotificationsVm));
        Shell.Current.CurrentItem = Shell.Current.Items[0].Items[notificationTabIndex];
    }

    public static async Task OpenMedicationPage(int medicationId)
    {
        var medication = await AppServices.Current.MedicationService.GetById(medicationId);
        var visitMedication = await AppServices.Current.VisitMedicationService.GetByMedicationId(medicationId);
        var booking = await AppServices.Current.BookingService.GetById(medication.BookingId);
        if ((booking.IsCompleted.HasValue && booking.IsCompleted.Value) || visitMedication.CompletedOn.HasValue)
            await OpenNotificationsPage();

        if (Shell.Current.CurrentPage is MedicationDetailPage)
            WeakReferenceMessenger.Default.Send(new MessagingEvents.MedicationPageUpdateReceivedMessage(true));
        else
        {
            var ongoingVm = ServiceLocator.GetService<OngoingVm>();
            var isReadOnly = ongoingVm?.OngoingDto?.Booking == null || ongoingVm?.OngoingDto?.Booking.Id != medication.BookingId;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Id", medicationId },
                { "BaseVisitId", visitMedication.VisitId },
                { "IsReadOnly", isReadOnly }
            };
            var medicationDetailViewPage = $"{nameof(MedicationDetailPage)}?";
            await Shell.Current.GoToAsync(medicationDetailViewPage, navigationParameter);
        }
    }

    public static async Task OpenErrorPage()
    {
        if (Shell.Current.Navigation.NavigationStack != null
            && Shell.Current.Navigation.NavigationStack.Any(x =>
                x != null && x.GetType().Name == nameof(ErrorPage)))
            return;

        await Shell.Current.GoToAsync(nameof(ErrorPage));
    }
}