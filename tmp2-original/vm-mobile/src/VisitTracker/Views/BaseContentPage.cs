namespace VisitTracker;

/// <summary>
/// BaseContentPage class for all ContentPages in the application.
/// It provides common properties and methods for handling navigation, access control, and UI state.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseContentPage<T> : ReactiveContentPage<T> where T : BaseVm
{
    /// <summary>
    /// Checks access control for the page when it appears.
    /// This method is called when the page is displayed and is responsible for checking if the user has access to the page.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!AccessControlExcludeList.Contains(typeof(T).Name) &&
            !AccessControlAllowList[AppData.Current.CurrentProfile.Type].Contains(typeof(T).Name))
        {
            var stack = Shell.Current.Navigation.NavigationStack.ToArray();
            Shell.Current.Navigation.RemovePage(stack.LastOrDefault());

            await Application.Current.MainPage.ShowSnackbar(Messages.NoAccessPage, false);
            await Navigation.PopAsync();
            return;
        }
    }

    /// <summary>
    /// Handles the back button press event for the page.
    /// This method is called when the user presses the back button on the device or navigates back in the application.
    /// </summary>
    /// <returns></returns>
    protected override bool OnBackButtonPressed()
    {
        var navigationStack = Shell.Current?.Navigation?.NavigationStack;
        if (navigationStack != null && navigationStack.Count > 1)
        {
            Shell.Current.Navigation.PopAsync();
            return true;
        }
        else return false;
    }

    private List<string> AccessControlExcludeList => [
        nameof(LoaderVm),
        nameof(LoginVm),
        nameof(LoginProviderVm),
        nameof(LoginProviderQrScannerVm),
        nameof(ErrorVm),
    ];

    /// <summary>
    /// Access control allow list for different user types.
    /// This dictionary defines which pages are accessible to each user type in the application.
    /// </summary>
    private Dictionary<string, List<string>> AccessControlAllowList => new Dictionary<string, List<string>>
    {
        {
            EUserType.CAREWORKER.ToString(),
            [
                nameof(CareWorkerHomeVm),
                nameof(CareWorkerDashboardVm),
                nameof(ServiceUserDetailVm),
                nameof(CareWorkerDetailVm),
                nameof(BookingsVm),
                nameof(BookingEditVm),
                nameof(BookingDetailVm),
                nameof(FluidChartVm),
                nameof(MarChartVm),
                nameof(OngoingVm),
                nameof(TaskDetailVm),
                nameof(FluidDetailVm),
                nameof(MedicationDetailVm),
                nameof(BodyMapNotesPopupVm),
                nameof(BodyMapVm),
                nameof(MiscellaneousVm),
                nameof(MiscellaneousDetailVm),
                nameof(NotificationsVm),
                nameof(IncidentReportVm),
            ]
        },
        {
            EUserType.SUPERVISOR.ToString(),
            [
                nameof(SupervisorHomeVm),
                nameof(SupervisorDashboardVm),
                nameof(ServiceUsersVm),
                nameof(ServiceUserDetailVm),
                nameof(CareWorkersVm),
                nameof(CareWorkerDetailVm),
                nameof(BookingsVm),
                nameof(BookingEditVm),
                nameof(BookingDetailVm),
                nameof(FluidChartVm),
                nameof(MarChartVm),
                nameof(OngoingVm),
                nameof(TaskDetailVm),
                nameof(FluidDetailVm),
                nameof(MedicationDetailVm),
                nameof(BodyMapNotesPopupVm),
                nameof(BodyMapVm),
                nameof(MiscellaneousVm),
                nameof(MiscellaneousDetailVm),
                nameof(NotificationsVm),
                nameof(IncidentReportVm),
            ]
        },
        {
            EUserType.SERVICEUSER.ToString(),
            [
                nameof(ServiceUserHomeVm),
                nameof(ServiceUserDashboardVm),
                nameof(ServiceUserDetailVm),
                nameof(BookingsVm),
                nameof(BookingDetailVm),
                nameof(FluidChartVm),
                nameof(MarChartVm),
                nameof(OngoingVm),
                nameof(TaskDetailVm),
                nameof(FluidDetailVm),
                nameof(MedicationDetailVm),
                nameof(BodyMapNotesPopupVm),
                nameof(BodyMapVm),
                nameof(MiscellaneousVm),
                nameof(MiscellaneousDetailVm),
                nameof(NotificationsVm),
                nameof(IncidentReportVm),
            ]
        },
        {
            EUserType.NEXTOFKIN.ToString(),
            [
                nameof(ServiceUserHomeVm),
                nameof(ServiceUserDashboardVm),
                nameof(ServiceUserDetailVm),
                nameof(BookingsVm),
                nameof(BookingDetailVm),
                nameof(FluidChartVm),
                nameof(MarChartVm),
                nameof(OngoingVm),
                nameof(TaskDetailVm),
                nameof(FluidDetailVm),
                nameof(MedicationDetailVm),
                nameof(BodyMapNotesPopupVm),
                nameof(BodyMapVm),
                nameof(MiscellaneousVm),
                nameof(MiscellaneousDetailVm),
                nameof(NotificationsVm),
                nameof(IncidentReportVm),
            ]
        }
    };
}