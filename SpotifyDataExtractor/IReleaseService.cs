using SpotifyDataExtractor.Models.Releases;

namespace SpotifyDataExtractor;

public interface IReleaseService
{
    Task<List<Release>> Get(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<List<Release>> Get(string artistId, CancellationToken cancellationToken = default);
}