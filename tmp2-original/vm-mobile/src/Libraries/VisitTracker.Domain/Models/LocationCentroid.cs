namespace VisitTracker.Domain;

public class LocationCentroid : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public string LocationClass { get; set; }
    public string DeviceInfo { get; set; }

    public int ServiceUserId { get; set; }

    public double CalibratedLatitude { get; set; }
    public double CalibratedLongitude { get; set; }
    public double FingerprintLatitude { get; set; }
    public double FingerprintLongitude { get; set; }

    public double StdDev { get; set; }
}