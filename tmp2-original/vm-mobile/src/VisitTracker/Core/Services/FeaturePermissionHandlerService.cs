namespace VisitTracker;

/// <summary>
/// FeaturePermissionHandlerService is a service that handles feature permissions in the application.
/// It provides methods to check, request, and open settings for various feature permissions.
/// </summary>
public partial class FeaturePermissionHandlerService
{
    public partial bool CheckPermission(EFeaturePermissionOrderedType type);

    public partial Task<bool> CheckEnabled(EFeaturePermissionOrderedType type);

    public partial void RequestPermission(EFeaturePermissionOrderedType type);

    public partial void RequestEnabled(EFeaturePermissionOrderedType type);

    public partial void OpenSettingsPage();

    /// <summary>
    /// CheckEverything checks the permissions and enabled status of all feature permissions in the application.
    /// It returns a list of errors if any permissions are not granted or enabled.
    /// </summary>
    /// <param name="forceRequest">to force the request if denied previously</param>
    /// <returns>list of ErrorDto that needs permission/access</returns>
    public async Task<List<ErrorDto>> CheckEverything(bool forceRequest = false)
    {
        if (AppData.Current.CurrentProfile == null) return null;
        var errors = new List<ErrorDto>();

        var featurePermissionTypes = (EFeaturePermissionOrderedType[])Enum.GetValues(typeof(EFeaturePermissionOrderedType));
        foreach (var featurePermissionType in featurePermissionTypes)
        {
            if (errors.Any(x => x.Type == EFeaturePermissionOrderedType.Internet)
                && featurePermissionType == EFeaturePermissionOrderedType.ClockTampered)
                continue;

            if (!CheckPermission(featurePermissionType))
                errors.Add(new ErrorDto
                {
                    Error = Messages.PermissionErrorMessage[featurePermissionType.ToString()],
                    Type = featurePermissionType,
                    ExecuteCommand = ReactiveCommand.Create<EFeaturePermissionOrderedType>(RequestPermission, outputScheduler: RxApp.MainThreadScheduler)
                });

            if (!await App.FeaturePermissionHandlerService.CheckEnabled(featurePermissionType))
                errors.Add(new ErrorDto
                {
                    Error = Messages.EnabledErrorMessage[featurePermissionType.ToString()],
                    Type = featurePermissionType,
                    ExecuteCommand = ReactiveCommand.Create<EFeaturePermissionOrderedType>(RequestEnabled, outputScheduler: RxApp.MainThreadScheduler)
                });
        }

        if (forceRequest) errors.ForEach(x => x.ExecuteCommand.Execute());

        return errors;
    }
}