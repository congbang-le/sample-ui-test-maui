namespace VisitTracker;

/// <summary>
/// FingerprintService is a service that manages the fingerprinting process for service users.
/// It provides methods to start and stop fingerprinting, process and upload fingerprinting data.
/// </summary>
public class FingerprintService
{
    public ServiceUserFp ServiceUserFp { get; set; }
    public int ServiceUserId { get; set; }

    public int TotalReadings { get; set; }

    public Timer FpTimer { get; set; }

    public FingerprintService()
    {
        FpTimer = new Timer();
    }

    /// <summary>
    /// Starts the fingerprinting process for a given service user ID.
    /// It initializes the fingerprinting service, deletes any existing fingerprint data, and starts the fingerprinting process.
    /// </summary>
    /// <param name="serviceUserId">Service user ID to start fingerprint</param>
    /// <returns></returns>
    public async Task StartFingerprint(int serviceUserId)
    {
        try
        {
            ServiceUserId = serviceUserId;
            await AppServices.Current.ServiceUserFpService.DeleteAll();
            ServiceUserFp = await AppServices.Current.ServiceUserFpService.InsertOrReplace(new ServiceUserFp
            {
                ServiceUserId = serviceUserId,
                StartedOn = DateTimeExtensions.NowNoTimezone(),
                MachineInfo = AppServices.Current.AppPreference.DeviceInfo,
                DeviceInfo = DeviceInfo.Platform.ToString(),
            });

            App.SupervisorLocationService.StartFingerprint();
            OnStarted(true);
        }
        catch (Exception ex)
        {
            await SystemHelper.Current.TrackError(ex);
            OnStarted(false);
        }
    }

    /// <summary>
    /// Stops the fingerprinting process for a given service user ID.
    /// It stops the fingerprinting service, deletes any existing fingerprint data, and syncs the data with the server.
    /// </summary>
    /// <param name="isForced">force stop</param>
    /// <returns></returns>
    public async Task StopFingerprint(bool isForced = false)
    {
        try
        {
            App.SupervisorLocationService.StopFingerprint();
            if (isForced) await OnCompleted(false);
            else
            {
                var fpId = ServiceUserFp.Id.ToString();
                ServiceUserFp = await AppServices.Current.ServiceUserFpService.InsertOrReplace(ServiceUserFp);
                var Locations = await AppServices.Current.LocationService.GetAllByServiceUserFp(ServiceUserFp.Id);
                await AppServices.Current.SyncService.InsertOrReplace(
                    new Sync
                    {
                        Identifier = Constants.SyncFpAction,
                        MetaData = fpId,
                        Content = JsonExtensions.Serialize(new
                        {
                            ServiceUserFp,
                            Locations
                        })
                    }
                );
                await AppServices.Current.LocationService.DeleteAllByVisitEmpty();
                await AppServices.Current.ServiceUserFpService.DeleteAll();

                var arg = await AppServices.Current.SyncService.SyncData();
                await AppServices.Current.SyncService.DeleteAllBySyncData();

                if (arg.Item1 != null && arg.Item1.ToList().Any(i => i.Identifier == Constants.SyncFpAction))
                    await OnCompleted(arg.Item1.ToList().Any(i => i.MetaData == fpId));
                else await OnCompleted(false);
            }
        }
        catch (Exception ex)
        {
            await SystemHelper.Current.TrackError(ex);
            await OnCompleted(false);
        }
    }

    /// <summary>
    /// Persists the fingerprint record.
    /// </summary>
    /// <param name="sensorString"></param>
    /// <returns></returns>
    public async Task UpdateFingerprint(string sensorString)
    {
        ServiceUserFp.SensorsNotFound = sensorString;
        ServiceUserFp = await AppServices.Current.ServiceUserFpService.InsertOrReplace(ServiceUserFp);
    }

    /// <summary>
    /// Processes the location data for a given visit location.
    /// It inserts or replaces the location data in the database and updates the total readings count.
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
    public async Task ProcessLocation(VisitLocation loc)
    {
        await AppServices.Current.LocationService.InsertOrReplace(loc);

        TotalReadings++;
        WeakReferenceMessenger.Default.Send(new MessagingEvents.FingerprintProgressMessage((float)TotalReadings / Constants.FpTotalReadings));
    }

    /// <summary>
    /// Initiates the timeout timer for fingerprinting.
    /// It sets the timer interval to the specified timeout duration and stops the fingerprinting process if the timeout occurs.
    /// </summary>
    private void InitiateTimeoutTimer()
    {
        FpTimer.Interval = Constants.FpGpsTimeoutInMs;
        FpTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
         MainThread.InvokeOnMainThreadAsync(async () =>
         {
             if (ServiceUserFp != null && App.IsFingerprinting)
             {
                 FpTimer.Stop();
                 await StopFingerprint();
             }
         });
        FpTimer.Start();
    }

    /// <summary>
    /// Callback method for when the fingerprinting process starts.
    /// It updates the preferences and sends a message indicating the fingerprinting status.
    /// </summary>
    /// <param name="isSuccess"></param>
    private void OnStarted(bool isSuccess)
    {
        Preferences.Default.Set(Constants.PrefKeyLocationFpUpdates, isSuccess);
        if (isSuccess) Preferences.Default.Set(Constants.PrefKeyOngoingFpServiceUserId, ServiceUserFp.ServiceUserId);
        else Preferences.Default.Remove(Constants.PrefKeyOngoingFpServiceUserId);

        App.IsFingerprinting = isSuccess;
        App.IsTracking = false;

        TotalReadings = 0;

        if (isSuccess) InitiateTimeoutTimer();
        else
        {
            App.SupervisorLocationService.StopFingerprint();
            ServiceUserFp = null;
        }

        WeakReferenceMessenger.Default.Send(new MessagingEvents.FingerprintStartedMessage(isSuccess));
    }

    /// <summary>
    /// Callback method for when the fingerprinting process is completed.
    /// It updates the preferences, stops the fingerprinting service, and syncs the data with the server.
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <returns></returns>
    private async Task OnCompleted(bool isSuccess)
    {
        Preferences.Default.Set(Constants.PrefKeyLocationFpUpdates, false);
        Preferences.Default.Remove(Constants.PrefKeyOngoingFpServiceUserId);

        App.IsFingerprinting = false;
        App.IsTracking = false;

        TotalReadings = 0;
        ServiceUserFp = null;
        FpTimer.Stop();

        await AppServices.Current.ServiceUserService.SyncServiceUser(ServiceUserId);
        ServiceUserId = default;
        WeakReferenceMessenger.Default.Send(new MessagingEvents.FingerprintCompletedMessage(isSuccess));
    }
}