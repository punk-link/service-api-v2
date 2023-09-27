namespace Core.Utils.Time;

public interface ITimeProvider
{
    public DateTime UtcNow { get; }
}