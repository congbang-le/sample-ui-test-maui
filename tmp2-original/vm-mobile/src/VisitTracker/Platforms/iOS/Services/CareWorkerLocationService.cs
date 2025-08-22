using CoreLocation;
using CoreMotion;
using Foundation;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using UIKit;

namespace VisitTracker;

public partial class CareWorkerLocationService : NSObject, ICLLocationManagerDelegate
{
    private double DrCurrentLat = 0;
    private double DrCurrentLon = 0;
    private readonly int LastNLocations = 10;
    private LimitedQueue<VisitLocation> AllLocationList;
    public LimitedQueue<double> AzimuthBuffer = new LimitedQueue<double>(Constants.AzimuthBufferCapacity);
    public List<CMPedometerData> DrActivationBuffer = new List<CMPedometerData>();

    private DateTime? iOSPersistLocTime;
    private bool DrIsEnabled = false;
    private int DrLastDistance = 0;
    private double DrStartLat = 0;
    private double DrStartLon = 0;
    private List<VisitStepCount> DrSteps;

    private double lastAzimuth;

    public int LocationCount = 0;
    protected CLLocationManager LocationManager;
    protected CMPedometer Pedometer;

    public CareWorkerLocationService()
    {
        AllLocationList = new LimitedQueue<VisitLocation>(LastNLocations);

        Pedometer = new CMPedometer();
        LocationManager = new CLLocationManager()
        {
            DesiredAccuracy = CLLocation.AccuracyNearestTenMeters,
            Delegate = this,
            DistanceFilter = CLLocationDistance.FilterNone,
            PausesLocationUpdatesAutomatically = false
        };

        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            LocationManager.RequestAlwaysAuthorization();

        if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            LocationManager.AllowsBackgroundLocationUpdates = true;
    }

    public partial void StartNormalMode()
    {
        LocationManager.StopUpdatingLocation();
        LocationManager.StopMonitoringSignificantLocationChanges();

        LocationManager.StartUpdatingLocation();

        if (CLLocationManager.SignificantLocationChangeMonitoringAvailable)
            LocationManager.StartMonitoringSignificantLocationChanges();
    }

    public partial void StopNormalMode()
    {
        StopTrackingMode();

        LocationManager.StopUpdatingLocation();
        LocationManager.StopMonitoringSignificantLocationChanges();
    }

    public partial void StartTrackingMode(double lat, double lon)
    {
        DrIsEnabled = false;
        DrStartLat = lat;
        DrStartLon = lon;
        AzimuthBuffer.Clear();
        DrActivationBuffer.Clear();
        DrSteps = new List<VisitStepCount>();

        if (CLLocationManager.HeadingAvailable)
            LocationManager.StartUpdatingHeading();

        if (CMPedometer.IsStepCountingAvailable)
            Pedometer.StartPedometerUpdates(NSDate.Now, (data, error) =>
                {
                    if (!DrIsEnabled && error == null && data != null)
                    {
                        DrActivationBuffer.Add(data);

                        double startTime, endTime = data.EndDate.SecondsSinceReferenceDate;
                        int i = DrActivationBuffer.Count - 1;
                        while (i > 0)
                        {
                            startTime = DrActivationBuffer[i - 1].StartDate.SecondsSinceReferenceDate;

                            if (endTime - startTime > Constants.DrActivationMaxNSeconds)
                                break;

                            if (endTime - startTime >= Constants.DrActivationNSeconds &&
                                endTime - startTime <= Constants.DrActivationMaxNSeconds
                                && DrActivationBuffer.TakeLast(DrActivationBuffer.Count - i + 1)
                                    .Sum(x => (int)x.NumberOfSteps) >= Constants.DrActivationNSteps)
                            {
                                DrIsEnabled = true;
                                DrActivationBuffer.Clear();
                                break;
                            }

                            i--;
                        }
                    }

                    if (DrIsEnabled)
                        MainThread.InvokeOnMainThreadAsync(() => CheckAndPersistStepCount(data));
                    else if (error != null)
                        Toast.Make(error.Description, ToastDuration.Long);
                });
    }

    private partial void StopTrackingMode()
    {
        DrIsEnabled = false;

        Pedometer.StopPedometerUpdates();
        LocationManager.StopUpdatingHeading();
    }

    [Export("locationManager:didUpdateLocations:")]
    public void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
    {
        try
        {
            var location = locations?.LastOrDefault();
            if (location != null) PersistLocation(location, ELocationClass.A);
        }
        catch (Exception ex)
        {
            _ = SystemHelper.Current.TrackError(ex);
        }
    }

    [Export("locationManager:didUpdateHeading:")]
    public void HeadingUpdated(CLLocationManager manager, CLHeading heading)
    {
        try
        {
            AzimuthBuffer.Enqueue(heading.HeadingAccuracy);
        }
        catch (Exception ex)
        {
            _ = SystemHelper.Current.TrackError(ex);
        }
    }

    [Export("locationManager:didFailWithError:")]
    public void Failed(CLLocationManager manager, NSError error)
    {
        Toast.Make(error.Description, ToastDuration.Long);
    }

    public void PersistLocation(CLLocation location, ELocationClass locationClass)
    {
        var dbLocation = new VisitLocation
        {
            Latitude = location.Coordinate.Latitude.ToString(),
            Longitude = location.Coordinate.Longitude.ToString(),
            LocationAccuracy = location.HorizontalAccuracy,
            Altitude = location.Altitude,
            Heading = location.Course,
            HeadingAccuracy = location.CourseAccuracy,
            Speed = location.Speed,
            LocationClass = locationClass.ToString(),
            Platform = (int)EPlatform.iOS,
            OnTime = location.Timestamp.ToDateTime().ToLocalTime(),
            Provider = EPlatform.iOS.ToString()
        };

        if (AppServices.Current.CareWorkerTrackerService.OnGoingBooking != null && AppServices.Current.CareWorkerTrackerService.OnGoingBookingDetailId > 0)
        {
            dbLocation.VisitId = AppServices.Current.CareWorkerTrackerService.Visit.Id;

            AllLocationList.Enqueue(dbLocation);

            var frequency = Constants.TrackingModeInterval1hrInMs;
            var bookingPeriod = (AppServices.Current.CareWorkerTrackerService.OnGoingBooking.EndTime - AppServices.Current.CareWorkerTrackerService.OnGoingBooking.StartTime).TotalMinutes;
            if (bookingPeriod <= 60) frequency = Constants.TrackingModeInterval1hrInMs;
            else if (bookingPeriod <= 240) frequency = Constants.TrackingModeInterval2hrInMs;
            else if (bookingPeriod <= 720) frequency = Constants.TrackingModeInterval4hrInMs;
            else frequency = Constants.TrackingModeInterval12hrInMs;

            if (iOSPersistLocTime == null || (DateTimeExtensions.NowNoTimezone() - iOSPersistLocTime.Value).TotalMilliseconds >= frequency)
            {
                iOSPersistLocTime = dbLocation.OnTime;
                if ((dbLocation.OnTime - iOSPersistLocTime.Value).TotalMilliseconds > frequency)
                    dbLocation = AllLocationList.OrderByDescending(i => i.OnTime).LastOrDefault();
            }
            else return;
        }

        _ = AppServices.Current.CareWorkerTrackerService.ProcessLocation(dbLocation);
    }

    public void PersistStepCount(VisitStepCount stepCount)
    {
        _ = AppServices.Current.CareWorkerTrackerService.ProcessStepCount(stepCount);
    }

    public void CheckAndPersistStepCount(CMPedometerData stepData)
    {
        if (stepData == null) return;

        double AvgAzimuth = GetAverageAzimuthCleared();
        DrUpdateLocation((int)stepData.Distance - DrLastDistance, AvgAzimuth);

        var CurrentStep = new VisitStepCount
        {
            OnTime = stepData.EndDate.ToDateTime().ToLocalTime(),
            CountTillNow = (int)stepData.NumberOfSteps,
            DistanceTillNow = (int)stepData.Distance,
            AzimuthNow = (float)AvgAzimuth,
            Latitude = DrCurrentLat,
            Longitude = DrCurrentLon
        };
        DrSteps.Add(CurrentStep);

        PersistStepCount(CurrentStep);
        DrLastDistance = (int)stepData.Distance;
    }

    public double GetAverageAzimuthCleared()
    {
        if (!AzimuthBuffer.Any()) return lastAzimuth;
        double AvgAz = Math.Atan2(AzimuthBuffer.Sum(Math.Sin), AzimuthBuffer.Sum(Math.Cos)) *
                       (180.0 / Math.PI);
        AzimuthBuffer.Clear();
        lastAzimuth = (AvgAz + 360.0) % 360.0;
        return (AvgAz + 360.0) % 360.0;
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
}