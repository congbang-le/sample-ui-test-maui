namespace VisitTracker.Domain;

public class BookingTask : RealmObject, IBaseModel
{
    [PrimaryKey]
    public int Id { get; set; }

    public int BookingId { get; set; }

    public string Title { get; set; }
    public short Order { get; set; }
}