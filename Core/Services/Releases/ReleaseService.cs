using Core.Converters.Spotify;
using Core.Data;
using Core.Data.Releases;
using Core.Models.Releases;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Core.Services.Releases;

public sealed class ReleaseService : IReleaseService
{
    public ReleaseService(Context context, SpotifyDataExtractor.IReleaseService spotifyReleaseService)
    {
        _context = context;
        _spotifyReleaseService = spotifyReleaseService;
    }


    public async Task<Result<List<Data.Releases.Release>>> GetMissing(string spotifyArtistId, CancellationToken cancellationToken = default)
    {
        return await new Result()
            .Ensure(() => !string.IsNullOrWhiteSpace(spotifyArtistId), "Artist's spotify ID is empty.")
            .Bind(GetPresentReleaseSpotifyIds)
            .Bind(GetMissingSpotifyReleaseIds)
            .Bind(GetSpotifyReleases);


        async Task<Result<List<string>>> GetPresentReleaseSpotifyIds() 
            => await _context.Releases
                .Where(x => x.ReleaseArtists.Any(y => y.SpotifyId == spotifyArtistId))
                .Select(x => x.SpotifyId)
                .ToListAsync(cancellationToken);


        async Task<Result<List<string>>> GetMissingSpotifyReleaseIds(List<string> spotifyReleaseIds)
        {
            // TODO: check for failures and replace the error message
            var spotifyReleases = await _spotifyReleaseService.Get(spotifyArtistId, cancellationToken);

            var presentIds = spotifyReleaseIds.ToHashSet();
            var results = new ConcurrentBag<string>();
            Parallel.ForEach(spotifyReleases, x =>
            {
                if (presentIds.Contains(x.Id))
                    results.Add(x.Id);
            });

            return results.ToList();
        }


        async Task<Result<List<Data.Releases.Release>>> GetSpotifyReleases(List<string> spotifyReleaseIds)
        {
            var spotifyReleases = await _spotifyReleaseService.Get(spotifyReleaseIds, cancellationToken);
            return spotifyReleases.Select(x => x.ToDbRelease())
                .ToList();
        }
    }


    public async Task<List<SlimRelease>> GetSlim(int artistId)
    {


        return await _context.Releases
            .Where(x => x.Id == artistId)
            .Select(x => new SlimRelease())
            .ToListAsync();
    }

    private readonly Context _context;
    private readonly SpotifyDataExtractor.IReleaseService _spotifyReleaseService;
}
