namespace VisitTracker;

/// <summary>
/// SupervisorTrackerService is a service that manages the tracking of supervisors during their visits.
/// </summary>
public class SupervisorTrackerService
{
    private VisitStepCount LastDrStepCount { get; set; }
    public LocationCentroid GroundTruth { get; set; }

    public bool IsTrackingMode { get; set; }
    public bool? EnteredGeozone { get; set; }
    public bool? ExitedGeozone { get; set; }

    public SupervisorVisit SupervisorVisit { get; set; }

    public bool StopPersistence { get; set; }

    /// <summary>
    /// Initializes with all the necessary data needed for the sensing logic to run.
    /// </summary>
    /// <param name="supervisorVisitId">Supervisor Visit ID to start the sensing logic</param>
    /// <returns></returns>
    public async Task Initialize(int supervisorVisitId)
    {
        LastDrStepCount = null;

        IsTrackingMode = false;
        StopPersistence = false;

        SupervisorVisit = await AppServices.Current.SupervisorVisitService.GetById(supervisorVisitId);
        GroundTruth = await AppServices.Current.LocationCentroidService.GetGroundTruth(SupervisorVisit.ServiceUserId);
    }

    /// <summary>
    /// Uninitializes by resetting all the properties to their default values.
    /// </summary>
    private void Uninitialize()
    {
        LastDrStepCount = null;
        GroundTruth = null;

        IsTrackingMode = false;
        StopPersistence = false;

        SupervisorVisit = null;
    }

    /// <summary>
    /// Starts the normal mode of the Sensing Logic. Normal mode is the default mode of the Sensing Logic.
    /// It is used to track the Supervisor with lower frequency GPS singals and do not include the step count data.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartNormalMode()
    {
        try
        {
            App.SupervisorLocationService.StartNormalMode();

            OnStarted(true);
            return true;
        }
        catch (Exception ex)
        {
            await SystemHelper.Current.TrackError(ex);

            OnStarted(false);
            return false;
        }
    }

    /// <summary>
    /// Stops the normal mode of the Sensing Logic with a possible exit reason.
    /// Stopping the normal mode will stop the Tracking mode as well and upload the data to the server.
    /// It will also set the Visit status to completed and update the visit details in the database.
    /// </summary>
    /// <param name="exitReason">Possible termination reason</param>
    /// <returns></returns>
    public async Task StopNormalMode(ESensorExitReason exitReason)
    {
        //This indicates that the Sensing Logic is already in process of termination and ignoring all persistence operations.
        //This is to avoid multiple calls to the StopNormalMode method in case of multiple location updates.
        if (!StopPersistence)
        {
            StopPersistence = true;
            var isTampered = await AppServices.Current.TamperingService.IsTimeTampered();
            if (SupervisorVisit != null)
            {
                SupervisorVisit.TerminationReason = exitReason.ToString();
                SupervisorVisit.TerminatedOn = DateTimeExtensions.NowNoTimezone();

                if (isTampered && exitReason != ESensorExitReason.NO_UPLOAD_ACK_TIMEOUT)
                    SupervisorVisit.IsVisitTampered = true;
                SupervisorVisit = await AppServices.Current.SupervisorVisitService.InsertOrReplace(SupervisorVisit);

                var locations = await AppServices.Current.LocationService.GetBySupVisit(SupervisorVisit.Id);
                var stepCounts = await AppServices.Current.StepCountService.GetBySupVisit(SupervisorVisit.Id);

                await AppServices.Current.SyncService.DeleteByIdentifierId(SupervisorVisit.Id);
                await AppServices.Current.SyncService.InsertOrReplace(
                    new Sync
                    {
                        Identifier = Constants.SyncSupSensorData,
                        IdentifierId = SupervisorVisit.Id,
                        Content = JsonExtensions.Serialize(new SupervisorVisitReportSensorDto
                        {
                            SupervisorVisit = SupervisorVisit,
                            DoPostProcessing = SupervisorVisit.CompletedOn != null,
                            LocationList = locations?.ToList(),
                            StepCountList = stepCounts?.ToList()
                        })
                    }
                );

                await AppServices.Current.LocationService.DeleteAllBySup(SupervisorVisit.Id);
                await AppServices.Current.StepCountService.DeleteAll();
                var (syncList, response) = await AppServices.Current.SyncService.SyncData();
                await AppServices.Current.SyncService.DeleteAllBySyncData();

                if (response != null && response.FailIds != null && response.FailIds.Any())
                    await AppServices.Current.BackgroundService.StartBackgroundTimer(5, 60);
            }
        }

        StopTrackingMode();
    }

    /// <summary>
    /// Starts the tracking mode of the Sensing Logic. Tracking mode is used to track the Supervisor with higher frequency GPS signals and includes the step count data.
    /// This will stop the normal mode and start the tracking mode with the given latitude and longitude.
    /// </summary>
    /// <param name="lat">latitude</param>
    /// <param name="lon">longitude</param>
    private void StartTrackingMode(double lat, double lon)
    {
        if (IsTrackingMode)
            App.SupervisorLocationService.StopNormalMode();

        App.SupervisorLocationService.StartTrackingMode(lat, lon);
    }

    /// <summary>
    /// Stops the tracking mode of the Sensing Logic.
    /// </summary>
    /// <returns></returns>
    private bool StopTrackingMode()
    {
        App.SupervisorLocationService.StopNormalMode();
        OnCompleted(true);
        return true;
    }

    /// <summary>
    /// Processes the location data received from the platform specific implementations.
    /// </summary>
    /// <param name="loc"></param>
    /// <returns></returns>
    public async Task ProcessLocation(VisitLocation loc)
    {
        //This indicates that the Sensing Logic is already in process of termination and ignoring all persistence operations.
        if (StopPersistence)
            return;

        var now = DateTimeExtensions.NowNoTimezone();
        var location = await AppServices.Current.LocationService.InsertOrReplace(loc);

        if (location.LocationClass == ELocationClass.A.ToString())
        {
            var distance = Location.CalculateDistance(Convert.ToDouble(location.Latitude), Convert.ToDouble(location.Longitude),
                GroundTruth.CalibratedLatitude, GroundTruth.CalibratedLongitude, DistanceUnits.Kilometers) * 1000;

            if (EnteredGeozone == null || !EnteredGeozone.Value)
                EnteredGeozone = distance <= Constants.GeoBoundaryExternalInMetres;

            ExitedGeozone = distance >= (1.5 * Constants.GeoBoundaryStartVisitInMetres);
        }

        //Evaluate mode based on Class A location with best accuracy and minimum time to stay parameter
        if ((EnteredGeozone.HasValue && EnteredGeozone.Value) && !IsTrackingMode)
        {
            StartTrackingMode(Convert.ToDouble(location.Latitude), Convert.ToDouble(location.Longitude));
            IsTrackingMode = true;
            Preferences.Default.Set(Constants.PrefKeyOngoingBookingTrackingMode, IsTrackingMode);
        }

        // Uploaded reports and exiting normally
        if (IsTrackingMode && EnteredGeozone.HasValue && EnteredGeozone.Value && SupervisorVisit.CompletedOn != null
            && ExitedGeozone.HasValue && ExitedGeozone.Value)
            await StopNormalMode(ESensorExitReason.UPLOAD_NORMAL_EXIT);

        // Timeout after submission of visit reports without exiting geofence
        else if (IsTrackingMode && SupervisorVisit.CompletedOn != null
                && SupervisorVisit.CompletedOn.Value.AddMinutes(Constants.TimeoutVisitUploadInMins) < now)
            await StopNormalMode(ESensorExitReason.UPLOAD_NORMAL_TIMEOUT);

        // Visit Started time-out (done)
        else if (SupervisorVisit.StartedOn != null && SupervisorVisit.CompletedOn == null && now > SupervisorVisit.StartedOn.Value.AddMinutes(Constants.TimeoutVisitStartInMins * 2))
            await StopNormalMode(ESensorExitReason.NO_UPLOAD_SV_TIMEOUT);
    }

    /// <summary>
    /// Processes the step count data received from the platform specific implementations.
    /// It inserts or replaces the step count data in the database and updates the last step count.
    /// </summary>
    /// <param name="stepCount"></param>
    /// <returns></returns>
    public async Task ProcessStepCount(VisitStepCount stepCount)
    {
        if (StopPersistence)
            return;

        stepCount.SupervisorVisitId = SupervisorVisit.Id;
        LastDrStepCount = await AppServices.Current.StepCountService.InsertOrReplace(stepCount);
    }

    /// <summary>
    /// Updates the Visit object if any of the sensors are not found during the visit.
    /// </summary>
    /// <param name="sensorString"></param>
    /// <returns></returns>
    public async Task UpdateVisit(string sensorString)
    {
        SupervisorVisit.SensorsNotFound = sensorString;
        SupervisorVisit = await AppServices.Current.SupervisorVisitService.InsertOrReplace(SupervisorVisit);
    }

    /// <summary>
    /// Resumes the normal mode of the Sensing Logic after any possible crash.
    /// </summary>
    /// <returns></returns>
    public async Task ResumeNormalMode()
    {
        App.IsTracking = await StartNormalMode();
    }

    /// <summary>
    /// Starts the normal mode of the Sensing Logic by Start Visit process.
    /// </summary>
    /// <param name="supervisorVisitId"></param>
    /// <returns></returns>
    public async Task StartNormalModeBySV(int supervisorVisitId)
    {
        await Initialize(supervisorVisitId);
        App.IsTracking = await StartNormalMode();

        AppServices.Current.SupervisorTrackerService.EnteredGeozone = App.IsTracking;
        AppServices.Current.SupervisorTrackerService.IsTrackingMode = App.IsTracking;
        Preferences.Default.Set(Constants.PrefKeyOngoingBookingTrackingMode, App.IsTracking);
    }

    /// <summary>
    /// Callback method to handle the status of the Sensing logic.
    /// It updates the application state and sends a message to notify other components about the status.
    /// </summary>
    /// <param name="isSuccess"></param>
    private void OnStarted(bool isSuccess)
    {
        App.IsTracking = isSuccess;
        App.IsFingerprinting = false;

        Preferences.Default.Set(Constants.PrefKeyLocationUpdates, isSuccess);
        if (isSuccess) Preferences.Default.Set(Constants.PrefKeyOngoingSupervisorVisitId, SupervisorVisit.Id);
        else Preferences.Default.Remove(Constants.PrefKeyOngoingSupervisorVisitId);

        WeakReferenceMessenger.Default.Send(new MessagingEvents.SupervisorVisitStartedMessage(isSuccess));
    }

    /// <summary>
    /// Callback method to handle the completion of the Sensing logic.
    /// It updates the application state, removes preferences, and sends a message to notify other components about the completion status.
    /// </summary>
    /// <param name="isSuccess"></param>
    private void OnCompleted(bool isSuccess)
    {
        App.IsTracking = false;
        App.IsFingerprinting = false;

        Preferences.Default.Set(Constants.PrefKeyLocationUpdates, !isSuccess);
        Preferences.Default.Remove(Constants.PrefKeyShowBookingIncomplete);
        if (isSuccess) Preferences.Default.Remove(Constants.PrefKeyOngoingSupervisorVisitId);

        Uninitialize();

        WeakReferenceMessenger.Default.Send(new MessagingEvents.VisitCompletedMessage(isSuccess));
    }
}