namespace SpotifyDataExtractor;

public interface ISpotifyClient
{
    Task<List<T>> Get<T>(IEnumerable<string> urls, CancellationToken cancellationToken = default) where T : struct;
    Task<T> Get<T>(string url, CancellationToken cancellationToken = default) where T : struct;
}