namespace VisitTracker;

/// <summary>
/// AppData is a singleton class that holds application-wide data and settings.
/// It provides access to various services and data models used throughout the application.
/// </summary>
public class AppData
{
    public static AppData Current => ServiceLocator.GetService<AppData>();

    public IList<Code> Codes { get; private set; }
    public IList<VisitMessage> VisitMessages { get; private set; }

    public IList<ExternalLinkDto> ExternalLinks { get; set; }

    public Profile CurrentProfile { get; private set; }
    public Provider Provider { get; private set; }

    public AppData()
    { }

    /// <summary>
    /// Initializes the application data by fetching the necessary data from the services.
    /// This includes fetching codes, visit messages, the logged-in profile, and the logged-in provider.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        Current.Codes = await AppServices.Current.CodeService.GetAll();
        Current.VisitMessages = await AppServices.Current.VisitMessageService.GetAll();
        Current.CurrentProfile = await AppServices.Current.ProfileService.GetLoggedInProfileUser();
        Current.Provider = await AppServices.Current.ProviderService.GetLoggedInProvider();

        if (Provider != null)
        {
            Current.ExternalLinks = new List<ExternalLinkDto>();

            var userTypeUrl = Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER) ? "vscw" : "vssu";
            if (Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER)
                    || Current.CurrentProfile?.Type == nameof(EUserType.SERVICEUSER)
                    || Current.CurrentProfile?.Type == nameof(EUserType.NEXTOFKIN))
            {
                Current.ExternalLinks.Add(new ExternalLinkDto
                {
                    LinkType = EExternalLinkType.PROFILE,
                    Title = "Profile",
                    Icon = MaterialCommunityIconsFont.Account,
                    ServerUrl = SystemHelper.Current.GetUrl(Constants.TpUrlProfile),
                });
            }

            if (Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER))
            {
                Current.ExternalLinks.Add(new ExternalLinkDto
                {
                    LinkType = EExternalLinkType.LEAVE,
                    Title = "Leave",
                    Icon = MaterialCommunityIconsFont.Office,
                    ServerUrl = SystemHelper.Current.GetUrl(Constants.TpUrlApplyLeave),
                });

                Current.ExternalLinks.Add(new ExternalLinkDto
                {
                    LinkType = EExternalLinkType.AVAILABILITY,
                    Title = "Availability",
                    Icon = MaterialCommunityIconsFont.CalendarEdit,
                    ServerUrl = SystemHelper.Current.GetUrl(Constants.TpUrlAvailability),
                });
            }

            if (Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER)
                    || Current.CurrentProfile?.Type == nameof(EUserType.SERVICEUSER)
                    || Current.CurrentProfile?.Type == nameof(EUserType.NEXTOFKIN))
            {
                Current.ExternalLinks.Add(new ExternalLinkDto
                {
                    LinkType = EExternalLinkType.CONTACTS,
                    Title = "Contacts",
                    Icon = MaterialCommunityIconsFont.Contacts,
                    ServerUrl = SystemHelper.Current.GetUrl(Constants.TpUrlContacts),
                });

                Current.ExternalLinks.Add(new ExternalLinkDto
                {
                    LinkType = EExternalLinkType.BOOKING_PREFERENCES,
                    Title = "Booking Preferences",
                    Icon = MaterialCommunityIconsFont.CalendarCheck,
                    ServerUrl = SystemHelper.Current.GetUrl(Constants.TpUrlBookingPreferences),
                });
            }
            ;

            if (Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER))
            {
                Current.ExternalLinks.Add(new ExternalLinkDto
                {
                    LinkType = EExternalLinkType.TRAININGS,
                    Title = "Trainings",
                    Icon = MaterialCommunityIconsFont.School,
                    ServerUrl = SystemHelper.Current.GetUrl(Constants.TpUrlTraining),
                });
            }

            Current.ExternalLinks.Add(new ExternalLinkDto
            {
                LinkType = EExternalLinkType.LOG_REQUEST,
                Title = "Log Request",
                Icon = MaterialCommunityIconsFont.Send,
                ServerUrl = SystemHelper.Current.GetUrl(Constants.TpUrlLogRequest, true),
            });

            Current.ExternalLinks.Add(new ExternalLinkDto
            {
                LinkType = EExternalLinkType.SUBMIT_FORMS,
                Title = "Forms",
                Icon = MaterialCommunityIconsFont.Pencil,
                ServerUrl = SystemHelper.Current.GetUrl(Constants.TpUrlForms, true),
            });
        }
    }

    /// <summary>
    /// Clears the application data by setting all properties to null.
    /// This is useful for resetting the application state, for example, when logging out or switching users.
    /// </summary>
    public void Clear()
    {
        Current.Codes = null;
        Current.VisitMessages = null;
        Current.CurrentProfile = null;
        Current.Provider = null;
        Current.ExternalLinks = null;
    }
}