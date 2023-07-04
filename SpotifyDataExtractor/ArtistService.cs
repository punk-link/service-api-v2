using SpotifyDataExtractor.Models.Artists;
using System.Web;

namespace SpotifyDataExtractor;

public class ArtistService : IArtistService
{
    public ArtistService(ISpotifyClient spotifyClient)
    {
        _spotifyClient = spotifyClient;
    }


    public async Task<List<Artist>> Get(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        if (ids is null || !ids.Any())
            return Enumerable.Empty<Artist>().ToList();

        var urls = ids.Chunk(ArtistQueryLimit)
            .Select(chunk => {
                var concatinatedIds = string.Join(',', chunk);
                return $"artists?ids={concatinatedIds}";
            });

        return await _spotifyClient.Get<Artist>(urls, cancellationToken);
    }


    public async Task<Artist> Get(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return default;

        return await _spotifyClient.Get<Artist>($"artists/{id}", cancellationToken);
    }


    public async Task<List<SlimArtist>> Search(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<SlimArtist>().ToList();

        var encodedQuery = HttpUtility.UrlEncode(query);

        return await _spotifyClient.Get<SlimArtist>(new List<string>(1) { $"search?type=artist&limit=10&q={encodedQuery}" }, cancellationToken);
    }


    private const int ArtistQueryLimit = 50;

    private readonly ISpotifyClient _spotifyClient;
}
