using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.Lang;
using AndroidApp = Android.App.Application;
using Exception = System.Exception;
using Location = Android.Locations.Location;
using Math = System.Math;
using Notification = Android.App.Notification;
using Object = Java.Lang.Object;

namespace VisitTracker;

[Service(Label = nameof(CareWorkerLocationService), Enabled = true, Exported = true, ForegroundServiceType = ForegroundService.TypeLocation)]
[IntentFilter(new string[] { "com.artivis.vm.CareWorkerLocationService" })]
public partial class CareWorkerLocationService : Service
{
    #region --------Ongoing Notification Declarations--------

    private const string LocationPackageName = "com.artivis.vm.CareWorkerLocationService";
    public string Tag = nameof(CareWorkerLocationService);

    private Notification.Builder TrackerNotificationBuilder { get; set; }
    private string TrackerChannelId = "channel_tracker";
    private string TrackerChannelName = Constants.AppName + " - Tracking Service";
    private const int TrackerNotificationId = 75968462;
    private string TrackerNotificationTitle = Constants.AppName + " - Live";
    private string TrackerNotificationText = Constants.AppName + " is tracking your visits!";

    private NotificationManager NotificationManager;

    private IBinder Binder;
    private Handler ServiceHandler;


    #endregion --------Ongoing Notification Declarations--------

    #region --------Location-based Declarations--------

    public LocationManager LocationManager;
    private ILocationListener LocationListenerGps;
    private ILocationListener LocationListenerNetwork;
    public Timer NetworkLocationTimer;

    public ValueTuple<double, double>? LastGpsLocation;

    #endregion --------Location-based Declarations--------

    #region --------Dead Reckoning Declarations--------

    public IWindowManager WindowManager;
    private Sensor AccelerometerSensor;
    private Sensor RotationVectorSensor;
    private Sensor StepDetectorSensor;
    private SensorManager SensorManager;
    private CareWorkerSensorListener CareWorkerSensorListener;

    public List<VisitStepCount> DrSteps;
    public bool DrIsEnabled = false;
    public int DrStepCount = 0;
    public double DrStartLat = 0;
    public double DrStartLon = 0;
    public double DrCurrentLat = 0;
    public double DrCurrentLon = 0;
    public long DrLastStepDetectTime = 0;

    public double lastAzimuth = 0;

    // Step detection parameters to be changed as needed
    public const double STEP_THRESHOLD_HIGH = 11.2;

    public const double STEP_THRESHOLD_LOW = 10.8;
    public const double STEP_THRESHOLD_BOTTOM = 10.6;
    public const long WINDOW_DURATION_MS = 350;
    public const long DELTA_MS = 80;
    public const double STEP_LENGTH_IN_METERS = 0.67;

    public LimitedQueue<DtoSensorData> AccelBuffer =
        new LimitedQueue<DtoSensorData>(Constants.AccelerationBufferCapacity);

    public LimitedQueue<double> AzimuthBuffer = new LimitedQueue<double>(Constants.AzimuthBufferCapacity);

    #endregion --------Dead Reckoning Declarations--------

    #region --------Constructor--------

    public CareWorkerLocationService()
    {
        Binder = new CareWorkerLocationServiceBinder(this);
    }

    #endregion --------Constructor--------

    #region --------Ongoing Notifications Implementations--------

    public override void OnCreate()
    {
        InitializeNetworkTimer();

        LocationManager = (LocationManager)AndroidApp.Context.GetSystemService(LocationService);
        LocationListenerGps = new CareWorkerLocationListenerGpsImpl { Service = this };
        LocationListenerNetwork = new CareWorkerLocationListenerNetworkImpl { Service = this };

        WindowManager = AndroidApp.Context.GetSystemService(WindowService).JavaCast<IWindowManager>();
        SensorManager = (SensorManager)GetSystemService(SensorService);
        CareWorkerSensorListener = new CareWorkerSensorListener { Service = this };

        AccelerometerSensor = SensorManager.GetDefaultSensor(SensorType.Accelerometer);
        RotationVectorSensor = SensorManager.GetDefaultSensor(SensorType.RotationVector);
        StepDetectorSensor = SensorManager.GetDefaultSensor(SensorType.StepDetector);

        HandlerThread handlerThread = new HandlerThread(Tag);
        handlerThread.Start();
        ServiceHandler = new Handler(handlerThread.Looper);

        NotificationManager = (NotificationManager)GetSystemService(NotificationService);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var trackerChannel = new NotificationChannel(TrackerChannelId, TrackerChannelName, NotificationImportance.Max);
            NotificationManager.CreateNotificationChannel(trackerChannel);
        }

        var mainActivity = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(MainActivity)), PendingIntentFlags.Immutable);

        TrackerNotificationBuilder = new Notification.Builder(ApplicationContext, TrackerChannelId)
            .SetContentIntent(mainActivity)
            .SetContentText(TrackerNotificationText)
            .SetContentTitle(TrackerNotificationTitle)
            .SetOngoing(true)
            .SetAutoCancel(false)
            .SetSmallIcon(Resource.Mipmap.appicon)
            .SetWhen(JavaSystem.CurrentTimeMillis());

        if (Build.VERSION.SdkInt > BuildVersionCodes.R)
        {
            TrackerNotificationBuilder.SetForegroundServiceBehavior((int)NotificationForegroundService.Immediate);
        }
    }

    private void InitializeNetworkTimer()
    {
        NetworkLocationTimer = new Timer();
        NetworkLocationTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            MainThread.InvokeOnMainThreadAsync(() =>
                LocationManager.RequestSingleUpdate(LocationManager.NetworkProvider, LocationListenerNetwork, null));
    }

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        StartForeground(TrackerNotificationId, GetNotification(), ForegroundService.TypeLocation);

        return StartCommandResult.Sticky;
    }

    public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
    {
        base.OnConfigurationChanged(newConfig);
    }

    public override IBinder OnBind(Intent intent)
    {
        return Binder;
    }

    public override void OnRebind(Intent intent)
    {
        base.OnRebind(intent);
    }

    public override bool OnUnbind(Intent intent)
    {
        return true;
    }

    public override void OnDestroy()
    {
        ServiceHandler.RemoveCallbacksAndMessages(null);
    }

    private Notification GetNotification()
    {
        return TrackerNotificationBuilder.Build();
    }

    #endregion --------Ongoing Notifications Implementations--------

    #region --------Invocations--------

    public partial void StartNormalMode()
    {
        StartService(new Intent(Android.App.Application.Context, typeof(CareWorkerLocationService)));

        LocationManager.RemoveUpdates(LocationListenerGps);
        LocationManager.RemoveUpdates(LocationListenerNetwork);

        LocationManager.RequestLocationUpdates(LocationManager.GpsProvider,
            Constants.NormalModeGpsIntervalInMs,
            Constants.MinimumDistanceChangeInMeters,
            LocationListenerGps,
            Looper.MainLooper);

        if (NetworkLocationTimer == null) InitializeNetworkTimer();
        NetworkLocationTimer.Interval = Constants.NormalModeNetworkIntervalInMs;
        NetworkLocationTimer.Start();

        _ = AppServices.Current.CareWorkerTrackerService.UpdateVisit(GetSensorsString());
    }

    private string GetSensorsString()
    {
        var HasStepDetectorSensor = PackageManager?.HasSystemFeature(PackageManager.FeatureSensorStepDetector);
        var sensorsNotFound = HasStepDetectorSensor != null && HasStepDetectorSensor.Value ? "" : "NO_STEP_SENSOR\n";
        sensorsNotFound += RotationVectorSensor != null ? "" : "NO_ROTATION_VECTOR_SENSOR\n";
        return string.IsNullOrEmpty(sensorsNotFound) ? "All sensors found!" : sensorsNotFound;
    }

    public partial void StopNormalMode()
    {
        StopTrackingMode();

        LocationManager.RemoveUpdates(LocationListenerGps);
        LocationManager.RemoveUpdates(LocationListenerNetwork);

        //Check whether the resources are being held-back
        StopForeground(StopForegroundFlags.Remove);
        StopSelf();
    }

    public partial void StartTrackingMode(double lat, double lon)
    {
        // If no gps signal available then use teh MAUI location
        if (LastGpsLocation == null)
            LastGpsLocation = (lat, lon);
            
        if (NetworkLocationTimer != null)
        {
            NetworkLocationTimer.Stop();
            NetworkLocationTimer = null;
        }

        LocationManager.RemoveUpdates(LocationListenerGps);
        LocationManager.RemoveUpdates(LocationListenerNetwork);

        var frequency = Constants.TrackingModeInterval1hrInMs;
        var bookingPeriod = (AppServices.Current.CareWorkerTrackerService.OnGoingBooking.EndTime - AppServices.Current.CareWorkerTrackerService.OnGoingBooking.StartTime).TotalMinutes;
        if (bookingPeriod <= 60) frequency = Constants.TrackingModeInterval1hrInMs;
        else if (bookingPeriod <= 240) frequency = Constants.TrackingModeInterval2hrInMs;
        else if (bookingPeriod <= 720) frequency = Constants.TrackingModeInterval4hrInMs;
        else frequency = Constants.TrackingModeInterval12hrInMs;

        LocationManager.RequestLocationUpdates(LocationManager.GpsProvider,
            frequency,
            Constants.MinimumDistanceChangeInMeters,
            LocationListenerGps,
            Looper.MainLooper);

        LocationManager.RequestLocationUpdates(LocationManager.NetworkProvider,
            frequency,
            Constants.MinimumDistanceChangeInMeters,
            LocationListenerNetwork,
            Looper.MainLooper);

        AzimuthBuffer.Clear();

        DrIsEnabled = false;
        DrStepCount = 0;
        DrStartLat = lat;
        DrStartLon = lon;
        DrSteps = new List<VisitStepCount>();

        var HasStepDetectorSensor = PackageManager?.HasSystemFeature(PackageManager.FeatureSensorStepDetector);
        if (HasStepDetectorSensor != null && HasStepDetectorSensor.Value && StepDetectorSensor != null)
            SensorManager.RegisterListener(CareWorkerSensorListener, StepDetectorSensor, SensorDelay.Normal);
        else if (AccelerometerSensor != null)
            SensorManager.RegisterListener(CareWorkerSensorListener, AccelerometerSensor, SensorDelay.Game);

        if (RotationVectorSensor != null)
            SensorManager.RegisterListener(CareWorkerSensorListener, RotationVectorSensor, SensorDelay.Game);
    }

    private partial void StopTrackingMode()
    {
        LocationManager.RemoveUpdates(LocationListenerGps);
        LocationManager.RemoveUpdates(LocationListenerNetwork);
        SensorManager.UnregisterListener(CareWorkerSensorListener);

        DrStepCount = 0;
        DrIsEnabled = false;

        if (NetworkLocationTimer != null)
        {
            NetworkLocationTimer.Stop();
            NetworkLocationTimer = null;
        }
    }

    #endregion --------Invocations--------

    #region --------Dead Reckoning Implementations--------

    public async Task DetectStep(DtoSensorData e)
    {
        bool Plateau;
        long ThisTime,
            NowTime = JavaSystem.CurrentTimeMillis() - SystemClock.ElapsedRealtime() + (e.Timestamp / 1000000);
        int ci = 0, PeakIndex = -1, WindowStartIndex = 0, WindowEndIndex = 0;
        double AccelMag = 0,
            PeakValue = -999.9,
            LeftMin = 999.9,
            RightMin = 999.9,
            LeftDeltaBottom = 999.0,
            RightDeltaBottom = 999.0;

        var se_list = AccelBuffer.ToList();
        List<double> AccMagList = new List<double>(Constants.AccelerationBufferCapacity);

        // Calculate the magnitude of the acceleration vector
        foreach (DtoSensorData acd in se_list)
            AccMagList.Add(Math.Sqrt(acd.Values[0] * acd.Values[0] + acd.Values[1] * acd.Values[1] +
                                     acd.Values[2] * acd.Values[2]));

        // Smooth the magnitude values by taking 3-point moving average
        for (int i = 0; i < AccMagList.Count(); i++)
        {
            if (i < 2) continue;
            AccMagList[i] = (AccMagList[i] + AccMagList[i - 1] + AccMagList[i - 2]) / 3;
        }

        if ((NowTime - DrLastStepDetectTime) < WINDOW_DURATION_MS)
            return;

        // Detect the peak within a window from (NowTime - WINDOW_DURATION_MS - DELTA_MS) to (NowTime - DELTA_MS)
        for (int j = 0; j < AccMagList.Count(); j++)
        {
            ThisTime = JavaSystem.CurrentTimeMillis() - SystemClock.ElapsedRealtime() +
                       (se_list[j].Timestamp / 1000000);
            if (ThisTime < (NowTime - WINDOW_DURATION_MS - DELTA_MS)) // Ignore values more than WINDOW_DURATION_MS old
            {
                WindowStartIndex = ++ci;
                continue;
            }

            if ((NowTime - ThisTime) <= DELTA_MS)
                continue;

            AccelMag = AccMagList[j];
            if (AccelMag > PeakValue)
            {
                PeakValue = AccelMag;
                PeakIndex = ci;
            }

            ci++;
            WindowEndIndex = j;
        }

        // Find the minimum value in the window before the peak
        for (int i = WindowStartIndex; i < PeakIndex; i++)
        {
            AccelMag = AccMagList[i];
            if (AccelMag < LeftMin)
                LeftMin = AccelMag;
        }

        for (int j = PeakIndex + 1; j < WindowEndIndex; j++)
        {
            AccelMag = AccMagList[j];
            if (AccelMag < RightMin)
                RightMin = AccelMag;
        }

        // Consider intervals of duration DELTA_MS before and after the window, respectively.
        // The values in these intervals should never exceed STEP_THRESHOLD_LOW
        // Also, find the minimum values in these intervals
        Plateau = true;
        for (int j = 0; j < AccMagList.Count(); j++)
        {
            if (j >= WindowStartIndex && j <= WindowEndIndex) // Ignore values inside window
                continue;
            ThisTime = JavaSystem.CurrentTimeMillis() - SystemClock.ElapsedRealtime() +
                       (se_list[j].Timestamp / 1000000);
            if (ThisTime < (NowTime - (WINDOW_DURATION_MS + (2 * DELTA_MS))))
                continue;

            if (AccMagList[j] > STEP_THRESHOLD_LOW)
            {
                Plateau = false;
                break;
            }

            if (j < WindowStartIndex)
                if (AccMagList[j] < LeftDeltaBottom)
                    LeftDeltaBottom = AccMagList[j];

            if (j > WindowEndIndex)
                if (AccMagList[j] < RightDeltaBottom)
                    RightDeltaBottom = AccMagList[j];
        }

        if ((PeakValue > STEP_THRESHOLD_HIGH) && (PeakValue < (STEP_THRESHOLD_HIGH + 7.0)) &&
            (LeftMin < STEP_THRESHOLD_LOW) && (RightMin < STEP_THRESHOLD_LOW) && (Plateau == true)
            && (LeftDeltaBottom < STEP_THRESHOLD_BOTTOM) && (RightDeltaBottom < STEP_THRESHOLD_BOTTOM))
            await CheckAndPersistStepCount(e.Timestamp);
    }

    public async Task CheckAndPersistStepCount(long OnTime)
    {
        double AvgAzimuth = GetAverageAzimuthCleared();
        var CurrentStep = new VisitStepCount
        {
            OnTime = DateTimeOffset.UnixEpoch.AddMilliseconds(JavaSystem.CurrentTimeMillis() -
                SystemClock.ElapsedRealtime() + (OnTime / 1000000)).LocalDateTime,
            CountTillNow = ++DrStepCount,
            DistanceTillNow = (float)(Constants.StepLengthInMetres * DrStepCount),
            AzimuthNow = (float)AvgAzimuth,
            Latitude = DrCurrentLat,
            Longitude = DrCurrentLon
        };
        DrSteps.Add(CurrentStep);

        if (!DrIsEnabled && DrSteps.Count(x => x.OnTime > DateTimeExtensions.NowNoTimezone()
                .Subtract(new TimeSpan(0, 0, 0, Constants.DrActivationNSeconds))) >= Constants.DrActivationNSteps)
        {
            DrIsEnabled = true;
            DrStepCount = 1;

            CurrentStep.CountTillNow = DrStepCount;
            CurrentStep.DistanceTillNow = (float)(Constants.StepLengthInMetres * DrStepCount);

            DrStartLat = LastGpsLocation != null ? Convert.ToDouble(LastGpsLocation.Value.Item1) : DrStartLat;
            DrStartLon = LastGpsLocation != null ? Convert.ToDouble(LastGpsLocation.Value.Item2) : DrStartLon;

            DrSteps.Clear();
        }

        if (DrIsEnabled)
        {
            DrUpdateLocation(Constants.StepLengthInMetres, AvgAzimuth);

            CurrentStep.Latitude = DrCurrentLat;
            CurrentStep.Longitude = DrCurrentLon;
            await PersistStepCount(CurrentStep);

            if (OnTime != 0)
                DrLastStepDetectTime = JavaSystem.CurrentTimeMillis() - SystemClock.ElapsedRealtime() + (OnTime / 1000000);
        }
    }

    public void DrUpdateLocation(double dist, double dir)
    {
        // Convert to radians
        double oldLatitude = Math.PI * DrStartLat / 180;
        double oldLongitude = Math.PI * DrStartLon / 180;
        float currentDirection = (float)(Math.PI * dir / 180.0);

        double newLatitude = Math.Asin(Math.Sin(oldLatitude) * Math.Cos(dist / Constants.EarthRadius) +
                                       Math.Cos(oldLatitude) * Math.Sin(dist / Constants.EarthRadius) *
                                       Math.Cos(currentDirection));

        double newLongitude = oldLongitude + Math.Atan2(Math.Sin(currentDirection) *
                                                        Math.Sin(dist / Constants.EarthRadius)
                                                        * Math.Cos(oldLatitude), Math.Cos(dist / Constants.EarthRadius)
            - Math.Sin(oldLatitude) * Math.Sin(newLatitude));

        //Convert back to degrees
        DrCurrentLat = 180 * newLatitude / Math.PI;
        DrCurrentLon = 180 * newLongitude / Math.PI;

        DrStartLat = DrCurrentLat;
        DrStartLon = DrCurrentLon;
    }

    public double GetAverageAzimuthCleared()
    {
        if (!AzimuthBuffer.Any()) return lastAzimuth;

        // Averaging angles based on www.themathdoctors.org/averaging-angles/
        double AvgAz = Math.Atan2(AzimuthBuffer.Sum(x => Math.Sin(x)), AzimuthBuffer.Sum(x => Math.Cos(x))) *
                       (180.0 / Math.PI);

        AzimuthBuffer.Clear();

        // '(exp + 360) % 360.0' for converting (-180 to +180) degree scheme to (0 to 360) degree scheme
        lastAzimuth = (AvgAz + 360.0) % 360.0;
        return (AvgAz + 360.0) % 360.0;
    }

    #endregion --------Dead Reckoning Implementations--------

    #region --------Data Persistence--------

    public async Task PersistLocation(Location location, ELocationClass locationClass)
    {
        var dbLocation = new VisitLocation
        {
            Latitude = location.Latitude.ToString(),
            Longitude = location.Longitude.ToString(),
            LocationAccuracy = location.Accuracy,
            Altitude = location.Altitude,
            Heading = location.Bearing,
            Speed = location.Speed,
            Provider = location.Provider,
            LocationClass = locationClass.ToString(),
            Platform = (int)EPlatform.Android,
            OnTime = DateTimeOffset.UnixEpoch.AddMilliseconds(location.Time).LocalDateTime,
            IsSync = false
        };

        if (AppServices.Current.CareWorkerTrackerService.OnGoingBooking != null && AppServices.Current.CareWorkerTrackerService.OnGoingBookingDetailId > 0)
            dbLocation.VisitId = AppServices.Current.CareWorkerTrackerService.Visit.Id;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            dbLocation.AltitudeAccuracy = location.HasVerticalAccuracy ? location.VerticalAccuracyMeters : default;
            dbLocation.HeadingAccuracy = location.HasBearingAccuracy ? location.BearingAccuracyDegrees : default;
        }

        await AppServices.Current.CareWorkerTrackerService.ProcessLocation(dbLocation);
        if (locationClass == ELocationClass.A)
            LastGpsLocation = (location.Latitude, location.Longitude);
    }

    public async Task PersistStepCount(VisitStepCount stepCount)
    {
        await AppServices.Current.CareWorkerTrackerService.ProcessStepCount(stepCount);
    }

    #endregion --------Data Persistence--------
}

#region --------Listener Implementations--------

public class CareWorkerLocationListenerGpsImpl : Object, ILocationListener
{
    public CareWorkerLocationService Service { get; set; }

    public async void OnLocationChanged(Location location)
    {
        try
        {
            if (Service.NetworkLocationTimer != null)
            {
                Service.NetworkLocationTimer.Stop();
                Service.NetworkLocationTimer.Start();
            }

            await Service.PersistLocation(location, ELocationClass.A);
        }
        catch (Exception ex)
        {
            await SystemHelper.Current.TrackError(ex);
        }
    }

    public void OnProviderDisabled(string provider)
    {
        Toast.Make("Location permissions needed to run the app!", ToastDuration.Long);
    }

    public void OnProviderEnabled(string provider)
    {
    }

    public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
    {
    }
}

public class CareWorkerLocationListenerNetworkImpl : Object, ILocationListener
{
    public CareWorkerLocationService Service { get; set; }

    public async void OnLocationChanged(Location location)
    {
        try
        {
            await Service.PersistLocation(location, ELocationClass.B);
        }
        catch (Exception ex)
        {
            await SystemHelper.Current.TrackError(ex);
        }
    }

    public void OnProviderDisabled(string provider)
    {
        Toast.Make("Location permissions needed to run the app!", ToastDuration.Long);
    }

    public void OnProviderEnabled(string provider)
    {
    }

    public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
    {
    }
}

public class CareWorkerSensorListener : Object, ISensorEventListener
{
    public CareWorkerLocationService Service { get; set; }

    public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
    {
    }

    public async void OnSensorChanged(SensorEvent e)
    {
        try
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                var ad = new DtoSensorData { Timestamp = e.Timestamp, Values = e.Values.ToList() };
                Service.AccelBuffer.Enqueue(ad);
                if (Service.AccelBuffer.Count >= Constants.AccelerationBufferCapacity)
                    await Service.DetectStep(ad);
            }
            else if (e.Sensor.Type == SensorType.RotationVector)
            {
                float[] rotationMatrix = new float[9];
                SensorManager.GetRotationMatrixFromVector(rotationMatrix, e.Values.ToArray());
                var surfaceOrientation = Service.WindowManager.DefaultDisplay.Rotation;
                int matrixColumn = 0, sense = 0;
                switch (surfaceOrientation)
                {
                    case SurfaceOrientation.Rotation0:
                        matrixColumn = 0;
                        sense = 1;
                        break;

                    case SurfaceOrientation.Rotation90:
                        matrixColumn = 1;
                        sense = -1;
                        break;

                    case SurfaceOrientation.Rotation180:
                        matrixColumn = 0;
                        sense = -1;
                        break;

                    case SurfaceOrientation.Rotation270:
                        matrixColumn = 1;
                        sense = 1;
                        break;
                }

                double x = sense * rotationMatrix[matrixColumn];
                double y = sense * rotationMatrix[matrixColumn + 3];
                double azimuth = -Java.Lang.Math.Atan2(y, x); // in radians
                Service.AzimuthBuffer.Enqueue(azimuth);
            }
            else if (e.Sensor.Type == SensorType.StepDetector)
                await Service.CheckAndPersistStepCount(e.Timestamp);
        }
        catch (Exception ex)
        {
            await SystemHelper.Current.TrackError(ex);
        }
    }
}

#endregion --------Listener Implementations--------

#region --------Ongoing Notification Implementations--------

public class CareWorkerLocationServiceBinder : Binder
{
    public CareWorkerLocationServiceBinder(CareWorkerLocationService locationUpdatesService)
    {
        this.Service = locationUpdatesService;
    }

    public CareWorkerLocationService Service { get; }
}

public class CareWorkerLocationServiceConnection : Object, IServiceConnection
{
    public void OnServiceConnected(ComponentName name, IBinder service)
    {
        var binder = (CareWorkerLocationServiceBinder)service;
        App.CareWorkerLocationService = binder.Service;
        App.LocationUpdatesServiceBound = true;
    }

    public void OnServiceDisconnected(ComponentName name)
    {
        App.CareWorkerLocationService = null;
        App.LocationUpdatesServiceBound = false;
    }
}

#endregion --------Ongoing Notification Implementations--------