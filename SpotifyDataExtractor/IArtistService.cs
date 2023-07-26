using SpotifyDataExtractor.Models.Artists;

namespace SpotifyDataExtractor;

public interface IArtistService
{
    Task<List<Artist>> Get(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<Artist> Get(string id, CancellationToken cancellationToken = default);
    Task<List<SlimArtist>> Search(string query, CancellationToken cancellationToken = default);
}