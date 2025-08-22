namespace VisitTracker;

/// <summary>
/// LimitedQueue is a custom queue implementation that limits the number of items in the queue to a specified limit.
/// When the limit is reached, the oldest item in the queue is removed to make space for the new item. 
/// Used in sensing logic sensor data processing.
/// </summary>
/// <typeparam name="T"></typeparam>
public class LimitedQueue<T> : Queue<T>
{
    public int Limit { get; set; }

    public LimitedQueue(int limit) : base(limit)
    {
        Limit = limit;
    }

    public new void Enqueue(T item)
    {
        while (Count >= Limit)
            Dequeue();

        base.Enqueue(item);
    }
}

public class DtoSensorData
{
    public long Timestamp { get; set; }

    public List<float> Values { get; set; }
}