namespace VisitTracker;

/// <summary>
/// CareWorkerTrackerService is a service that manages the tracking of care workers during their visits.
/// </summary>
public class CareWorkerTrackerService
{
    private VisitStepCount LastDrStepCount { get; set; }
    public LocationCentroid GroundTruth { get; set; }

    public bool IsTrackingMode { get; set; }
    public bool? EnteredGeozone { get; set; }
    public bool? ExitedGeozone { get; set; }

    public Booking OnGoingBooking { get; set; }
    public int OnGoingBookingDetailId { get; set; }
    public int CareWorkerId { get; set; }
    public Visit Visit { get; set; }

    public Booking NextOrUpcomingBooking { get; set; }
    public int? NextOrUpcomingBookingDetailId { get; set; }

    private DateTime? PingLastNthMinUpload { get; set; }

    public bool StopPersistence { get; set; }

    /// <summary>
    /// Initializes with all the necessary data needed for the sensing logic to run.
    /// </summary>
    /// <param name="bookingDetailId">Booking Detail ID to start the sensing logic</param>
    /// <returns></returns>
    public async Task Initialize(int bookingDetailId)
    {
        LastDrStepCount = null;

        IsTrackingMode = false;
        StopPersistence = false;

        OnGoingBookingDetailId = bookingDetailId;

        Visit = await AppServices.Current.VisitService.GetByBookingDetailId(bookingDetailId);

        var bookingDetail = await AppServices.Current.BookingDetailService.GetById(bookingDetailId);
        OnGoingBooking = await AppServices.Current.BookingService.GetById(bookingDetail.BookingId);
        CareWorkerId = bookingDetail.CareWorkerId;

        GroundTruth = await AppServices.Current.LocationCentroidService.GetGroundTruth(OnGoingBooking.ServiceUserId);

        var nextBooking = await AppServices.Current.BookingService.GetNextBooking(OnGoingBooking.Id);
        if (nextBooking != null)
        {
            var upComingBookingDeatil = await AppServices.Current.BookingDetailService.GetByBookingId(nextBooking.Id);
            NextOrUpcomingBooking = nextBooking;
            NextOrUpcomingBookingDetailId = upComingBookingDeatil.Id;
        }
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

        OnGoingBooking = null;
        NextOrUpcomingBooking = null;
        OnGoingBookingDetailId = default;
        NextOrUpcomingBookingDetailId = default;
        Visit = null;
    }

    /// <summary>
    /// Starts the normal mode of the Sensing Logic. Normal mode is the default mode of the Sensing Logic.
    /// It is used to track the Care Worker with lower frequency GPS singals and do not include the step count data.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartNormalMode()
    {
        try
        {
            App.CareWorkerLocationService.StartNormalMode();

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
            if (OnGoingBooking != null)
            {
                var ongoingBookingDetail = await AppServices.Current.BookingDetailService.GetById(OnGoingBookingDetailId);
                if (ongoingBookingDetail.IsMaster
                        && exitReason != ESensorExitReason.NO_UPLOAD_SV_TIMEOUT && exitReason != ESensorExitReason.NO_UPLOAD_SV_TIMEOUT_RESUME
                        && exitReason != ESensorExitReason.NO_UPLOAD_ACK_TIMEOUT && exitReason != ESensorExitReason.NO_UPLOAD_ACK_TIMEOUT_RESUME)
                    OnGoingBooking = await AppServices.Current.BookingService.SetCurrentBookingStatus(OnGoingBooking.Id, ECodeType.BOOKING_STATUS, ECodeName.PROGRESS);

                var bv = await AppServices.Current.VisitService.GetByBookingDetailId(OnGoingBookingDetailId);
                bv.TerminationReason = exitReason.ToString();
                bv.TerminatedOn = DateTimeExtensions.NowNoTimezone();
                if (isTampered && exitReason != ESensorExitReason.NO_UPLOAD_ACK_TIMEOUT)
                    bv.IsVisitTampered = true;

                if (exitReason != ESensorExitReason.NO_UPLOAD_SV_TIMEOUT && exitReason != ESensorExitReason.NO_UPLOAD_ACK_TIMEOUT)
                    bv.VisitStatusId = AppData.Current.Codes.FirstOrDefault(x => x.Type == ECodeType.VISIT_STATUS.ToString() && x.Name == ECodeName.COMPLETED.ToString()).Id;
                Visit = await AppServices.Current.VisitService.SyncVisit(bv);

                var locations = await AppServices.Current.LocationService.GetAllByVisitId(bv.Id);
                var stepCounts = await AppServices.Current.StepCountService.GetAllByVisitId(bv.Id);

                await AppServices.Current.SyncService.DeleteByIdentifierId(bv.Id);
                await AppServices.Current.SyncService.InsertOrReplace(
                    new Sync
                    {
                        Identifier = Constants.SyncSensorData,
                        IdentifierId = bv.Id,
                        Content = JsonExtensions.Serialize(new VisitReportSensorDto
                        {
                            BookingDetailId = OnGoingBookingDetailId,
                            DoPostProcessing = Visit.CompletedOn != null,
                            Visit = bv,
                            LocationList = locations,
                            StepCountList = stepCounts
                        })
                    }
                );

                await AppServices.Current.LocationService.DeleteAllByServiceUserFpEmpty();
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
    /// Starts the tracking mode of the Sensing Logic. Tracking mode is used to track the Care Worker with higher frequency GPS signals and includes the step count data.
    /// This will stop the normal mode and start the tracking mode with the given latitude and longitude.
    /// </summary>
    /// <param name="lat">latitude</param>
    /// <param name="lon">longitude</param>
    private void StartTrackingMode(bool isForced, double lat, double lon)
    {
        try
        {
            if (IsTrackingMode)
                App.CareWorkerLocationService.StopNormalMode();

            App.CareWorkerLocationService.StartTrackingMode(lat, lon);
            if (isForced)
                OnStarted(true);
        }
        catch (Exception ex)
        {
            if (isForced)
                OnStarted(false);
            else
                throw ex;
        }
    }

    /// <summary>
    /// Stops the tracking mode of the Sensing Logic.
    /// </summary>
    /// <returns></returns>
    private bool StopTrackingMode()
    {
        App.CareWorkerLocationService.StopNormalMode();
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

        var isNextBookingInScope = Visit.CompletedOn != null && NextOrUpcomingBooking != null
                && NextOrUpcomingBooking.StartTime.AddMinutes(-Constants.NextBookingScopeInMins) <= now;
        var tempNextOrUpcomingBookingDetailId = NextOrUpcomingBookingDetailId;

        //ETA & Health Ping Calculation
        //This condition checks whether the device is connected to the internet and whether there is a need to ping the location.
        //If the device is connected to the internet and the last ping was more than N minutes ago, it pings the location.
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet && (Visit.CompletedOn == null || isNextBookingInScope)&&
            (PingLastNthMinUpload == null || PingLastNthMinUpload.Value.AddMinutes(Constants.PingEveryNMinUpload) < now))
        {
            PingLastNthMinUpload = now;

            await AppServices.Current.VisitService.PingLocation(new LastKnownDto
            {
                CareWorkerId = CareWorkerId,
                BookingDetailId = OnGoingBookingDetailId,
                DeviceInfo = DeviceInfo.Platform.ToString(),
                Latitude = (LastDrStepCount == null) ? location.Latitude : LastDrStepCount.Latitude.ToString(),
                Longitude = (LastDrStepCount == null) ? location.Longitude : LastDrStepCount.Longitude.ToString(),
                LocOnTime = (LastDrStepCount == null) ? location.OnTime : LastDrStepCount.OnTime,
            });
        }

        //Checks whether the Care Worker is exited or entered the geofence of the Service User home.
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
            StartTrackingMode(false, Convert.ToDouble(location.Latitude), Convert.ToDouble(location.Longitude));
            IsTrackingMode = true;
            Preferences.Default.Set(Constants.PrefKeyOngoingBookingTrackingMode, IsTrackingMode);
        }

        var normalModeStopped = false;
        // Uploaded reports and exiting normally
        if (IsTrackingMode && EnteredGeozone.HasValue && EnteredGeozone.Value && Visit.CompletedOn != null
            && ExitedGeozone.HasValue && ExitedGeozone.Value)
        {
            await StopNormalMode(ESensorExitReason.UPLOAD_NORMAL_EXIT);
            normalModeStopped = true;
        }

        // Timeout after submission of visit reports without exiting geofence
        else if (IsTrackingMode && Visit.CompletedOn != null
                && now > Visit.CompletedOn.Value.AddMinutes(Constants.TimeoutVisitUploadInMins))
        {
            await StopNormalMode(ESensorExitReason.UPLOAD_NORMAL_TIMEOUT);
            normalModeStopped = true;
        }

        // Acknowledge time-out (Visit not started - start visit button)
        else if (Visit.StartedOn == null && now > OnGoingBooking.EndTime)
        {
            await StopNormalMode(ESensorExitReason.NO_UPLOAD_ACK_TIMEOUT);
            normalModeStopped = true;
        }

        // Visit Started time-out (done)
        else if (Visit.StartedOn != null && Visit.CompletedOn == null
                && now > Visit.StartedOn.Value.AddMinutes(Constants.TimeoutVisitStartInMins
                    + (OnGoingBooking.EndTime - OnGoingBooking.StartTime).TotalMinutes))
        {
            await StopNormalMode(ESensorExitReason.NO_UPLOAD_SV_TIMEOUT);
            normalModeStopped = true;
        }

        if (normalModeStopped && isNextBookingInScope)
            await StartNormalModeByAck(tempNextOrUpcomingBookingDetailId.Value);
    }

    /// <summary>
    /// Processes the step count data received from the platform specific implementations.
    /// It inserts or replaces the step count data in the database and updates the last step count.
    /// </summary>
    /// <param name="stepCount"></param>
    /// <returns></returns>
    public async Task ProcessStepCount(VisitStepCount stepCount)
    {
        //This indicates that the Sensing Logic is already in process of termination and ignoring all persistence operations.
        if (StopPersistence)
            return;

        stepCount.VisitId = Visit.Id;
        LastDrStepCount = await AppServices.Current.StepCountService.InsertOrReplace(stepCount);
    }

    /// <summary>
    /// Updates the Visit object if any of the sensors are not found during the visit.
    /// </summary>
    /// <param name="sensorString"></param>
    /// <returns></returns>
    public async Task UpdateVisit(string sensorString)
    {
        var bookingVisit = await AppServices.Current.VisitService.GetByBookingDetailId(OnGoingBookingDetailId);
        bookingVisit.SensorsNotFound = sensorString;
        Visit = await AppServices.Current.VisitService.InsertOrReplace(bookingVisit);
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
    /// Starts the normal mode of the Sensing Logic by auto acknowledge process.
    /// </summary>
    /// <param name="bookingDetailId">Booking Detail ID to start Sensing logic</param>
    /// <returns>Visit object that is started</returns>
    public async Task<Visit> StartNormalModeByAck(int bookingDetailId)
    {
        //This is the initialization point of the Sensing Logic, hence calling the Initialize method to set up all the required data.
        await Initialize(bookingDetailId);

        var now = DateTimeExtensions.NowNoTimezone();
        if (Visit == null)
        {
            Visit = new Visit
            {
                BookingDetailId = bookingDetailId,
                IsCompleted = false,
                AcknowledgedOn = now,
                MachineInfo = AppServices.Current.AppPreference.DeviceInfo,
                DeviceInfo = DeviceInfo.Platform.ToString(),
                VisitStatusId = AppData.Current.Codes.FirstOrDefault(x => x.Type == ECodeType.VISIT_STATUS.ToString() && x.Name == ECodeName.ACKNOWLEDGED.ToString()).Id
            };
            Visit = await AppServices.Current.VisitService.SyncVisit(Visit);
        }

        App.IsTracking = await StartNormalMode();

        return Visit;
    }

    /// <summary>
    /// Starts the normal mode of the Sensing Logic by Start Visit process.
    /// </summary>
    /// <param name="bookingDetailId">Booking Detail ID to start Sensing logic</param>
    /// <param name="biometricDto">Biometric data</param>
    /// <returns></returns>
    public async Task<Visit> StartNormalModeBySV(int bookingDetailId, VisitBiometricDto biometricDto)
    {
        await Initialize(bookingDetailId);

        var now = DateTimeExtensions.NowNoTimezone();
        var currentDevicelocation = await GeolocationExtensions.GetBestLocationAsync();
        if (Visit == null)
            Visit = new Visit
            {
                BookingDetailId = bookingDetailId,
                IsCompleted = false,
                MachineInfo = AppServices.Current.AppPreference.DeviceInfo,
                DeviceInfo = DeviceInfo.Platform.ToString()
            };

        Visit.VisitStatusId = AppData.Current.Codes.FirstOrDefault(x => x.Type == ECodeType.VISIT_STATUS.ToString() && x.Name == ECodeName.PROGRESS.ToString()).Id;
        Visit.StartedOn = now;
        Visit.StartedLocation = $"{currentDevicelocation.Latitude},{currentDevicelocation.Longitude}";
        Visit = await AppServices.Current.VisitService.SyncVisit(Visit, biometricDto);

        StartTrackingMode(true, currentDevicelocation.Latitude, currentDevicelocation.Longitude);
        App.IsTracking = true;

        AppServices.Current.CareWorkerTrackerService.EnteredGeozone = App.IsTracking;
        AppServices.Current.CareWorkerTrackerService.IsTrackingMode = App.IsTracking;
        Preferences.Default.Set(Constants.PrefKeyOngoingBookingTrackingMode, App.IsTracking);

        return Visit;
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
        if (isSuccess) Preferences.Default.Set(Constants.PrefKeyOngoingBookingDetailId, OnGoingBookingDetailId);
        else Preferences.Default.Remove(Constants.PrefKeyOngoingBookingDetailId);

        WeakReferenceMessenger.Default.Send(new MessagingEvents.CareWorkerVisitStartedMessage((isSuccess, OnGoingBookingDetailId)));
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
        if (isSuccess) Preferences.Default.Remove(Constants.PrefKeyOngoingBookingDetailId);

        Uninitialize();

        WeakReferenceMessenger.Default.Send(new MessagingEvents.VisitCompletedMessage(isSuccess));
    }
}