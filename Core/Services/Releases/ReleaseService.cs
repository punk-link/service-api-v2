using Core.Converters.Artists;
using Core.Converters.Spotify;
using Core.Data;
using Core.Extensions.Logging;
using Core.Models.Releases;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Core.Services.Releases;

public sealed class ReleaseService : IReleaseService
{
    public ReleaseService(Context context, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ReleaseService>();

        _context = context;
    }


    public async Task<Result> Add(List<SpotifyDataExtractor.Models.Releases.Release> releases, DateTime timeStamp, CancellationToken cancellationToken = default)
    {
        return await Result.Success()
            .Bind(GetSpotifyArtistIds)
            .Bind(GetArtists)
            .Bind(AddReleases)
            .Bind(AddTracks);

        
        Result<List<string>> GetSpotifyArtistIds()
        {
            var artistIds = new ConcurrentBag<string>();
            Parallel.ForEach(releases, release => 
            {
                foreach (var artist in release.Artists)
                    artistIds.Add(artist.Id);

                foreach (var track in release.Tracks.Items)
                {
                    foreach (var artist in track.Artists)
                        artistIds.Add(artist.Id);
                }
            });

            return artistIds.Distinct()
                .ToList();
        }


        async Task<Result<Dictionary<string, Data.Artists.Artist>>> GetArtists(List<string> spotifyArtistIds) 
            => await _context.Artists
                .Where(x => spotifyArtistIds.Contains(x.SpotifyId))
                .Select(x => x.ToIdOnlyDbArtist())
                .ToDictionaryAsync(x => x.SpotifyId, x => x, cancellationToken);


        async Task<Result<(Dictionary<string, Data.Artists.Artist>, List<Data.Releases.Release>)>> AddReleases(Dictionary<string, Data.Artists.Artist> artistDict)
        {
            var dbReleases = new ConcurrentBag<Data.Releases.Release>();
            Parallel.ForEach(releases, release =>
            {
                var releaseArtists = new List<Data.Artists.Artist>(release.Artists.Count);
                foreach (var artist in release.Artists)
                {
                    if (!artistDict.TryGetValue(artist.Id, out var featuringArtist))
                    {
                        _logger.LogSpotifyArtistIdWasntFound(artist.Id);
                        continue;
                    }

                    releaseArtists.Add(featuringArtist);
                }

                var dbRelease = release.ToDbRelease(releaseArtists, timeStamp);
                dbReleases.Add(dbRelease);
            });

            await _context.Releases.AddRangeAsync(dbReleases, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return (artistDict, dbReleases.ToList());
        }


        async Task<Result> AddTracks((Dictionary<string, Data.Artists.Artist> ArtistDict, List<Data.Releases.Release> DbReleases) container)
        {
            var releaseDict = container.DbReleases.ToDictionary(x => x.SpotifyId, x => x);

            var dbTracks = new ConcurrentBag<Data.Releases.Track>();
            Parallel.ForEach(releases, release => 
            {
                if (!releaseDict.TryGetValue(release.Id, out var dbRelease))
                {
                    _logger.LogSpotifyReleaseIdWasntFound(release.Id);
                    return;
                }

                foreach (var track in release.Tracks.Items)
                {
                    var trackArtists = new List<Data.Artists.Artist>(track.Artists.Count);
                    foreach (var artist in track.Artists)
                    {
                        if (!container.ArtistDict.TryGetValue(artist.Id, out var featuringArtist))
                        {
                            _logger.LogSpotifyArtistIdWasntFound(artist.Id);
                            continue;
                        }

                        trackArtists.Add(featuringArtist);
                    }

                    dbTracks.Add(track.ToDbTrack(dbRelease, trackArtists, timeStamp));
                }
            });

            await _context.Tracks.AddRangeAsync(dbTracks, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }


    public async Task<List<SlimRelease>> GetSlim(int artistId, CancellationToken cancellationToken = default)
    {
        return await _context.Releases
            .Where(x => x.Id == artistId)
            .Select(x => new SlimRelease())
            .ToListAsync(cancellationToken);
    }


    private readonly Context _context;
    private readonly ILogger _logger;
}
