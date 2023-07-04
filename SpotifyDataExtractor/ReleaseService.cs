using SpotifyDataExtractor.Models.Releases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyDataExtractor;

public class ReleaseService : IReleaseService
{
    public ReleaseService(ISpotifyClient spotifyClient) 
    {
        _spotifyClient = spotifyClient;    
    }


    public async Task<List<Release>> Get(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        if (ids is null || !ids.Any())
            return Enumerable.Empty<Release>().ToList();

        var urls = ids.Chunk(ReleaseQueryLimit)
            .Select(chunk => {
                var concatinatedIds = string.Join(',', chunk);
                return $"albums?ids={concatinatedIds}";
            });

        return await _spotifyClient.Get<Release>(urls, cancellationToken);
    }


    public async Task<List<Release>> Get(string artistId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(artistId))
            return Enumerable.Empty<Release>().ToList();

        var results = new List<Release>();

        var skip = 0;
        while (true)
        {
            var urls = new List<string>(1) { $"artists/{artistId}/albums?limit={ReleaseQueryLimit}&offset={skip}" };
            var container = await _spotifyClient.Get<ReleaseContainer>(urls, cancellationToken);

            results.AddRange(container.First().Items);
            if (container.First().Next == string.Empty)
                break;

            skip += ReleaseQueryLimit;
        }

        return results;
    }


    private const int ReleaseQueryLimit = 20;

    private readonly ISpotifyClient _spotifyClient;
}
