namespace Core.Utils.Time;

public class TimeProvider : ITimeProvider
{
    public DateTime UtcNow
        => DateTime.UtcNow;
}