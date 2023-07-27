using Core.Converters.Artists;
using Core.Converters.Spotify;
using Core.Data;
using Core.Data.Artists;
using Core.Extensions;
using Core.Models.Artists;
using Core.Models.Labels;
using Core.Models.Labels.Validations;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Artist = Core.Models.Artists.Artist;

namespace Core.Services.Artists;

public sealed class ArtistService : IArtistService
{
    public ArtistService(Context context, 
        ILoggerFactory loggerFactory,
        SpotifyDataExtractor.IArtistService spotifyArtistService, 
        SpotifyDataExtractor.IReleaseService spotifyReleaseService)
    {
        _logger = loggerFactory.CreateLogger<ArtistService>();

        _context = context;
        _spotifyArtistService = spotifyArtistService;
        _spotifyReleaseService = spotifyReleaseService;
    }


    public async Task<Result<Artist>> Add(ManagerContext managerContext, string spotifyId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await Result.Success()
            .Ensure(() => !string.IsNullOrWhiteSpace(spotifyId), "Artist's spotify ID is empty.")
            .Bind(GetExistingArtist)
            .Bind(AddOrUpdateArtist)
            .Bind(AddMissingReleases)
            .Bind(GetArtist);


        async Task<Result<Data.Artists.Artist?>> GetExistingArtist() 
            => await _context.Artists
                .FirstOrDefaultAsync(x => x.SpotifyId == spotifyId, cancellationToken);


        async Task<Result<string>> AddOrUpdateArtist(Data.Artists.Artist? dbArtist)
        {
            if (dbArtist is null)
                return await AddArtist();

            var validator = new ManagerContextValidator();
            var validationResult = validator.ValidateArtistBelongsToLabel(managerContext, dbArtist.LabelId);
            if (validationResult is not null)
                return Result.Failure<string>(validationResult.ToCombinedMessage());

            return await UpdateArtist(dbArtist);


            async Task<Result<string>> AddArtist()
            {
                // TODO: check for failures and replace the error message
                var spotifyArtist = await _spotifyArtistService.Get(spotifyId, cancellationToken);
                if (spotifyArtist == default)
                    return Result.Failure<string>("Failure");

                var dbArtist = spotifyArtist.ToDbArtist(managerContext.LabelId, now);
                await _context.Artists.AddAsync(dbArtist, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return dbArtist.SpotifyId;
            }


            async Task<Result<string>> UpdateArtist(Data.Artists.Artist dbArtist)
            {
                dbArtist.LabelId = managerContext.LabelId;
                dbArtist.Updated = now;

                _context.Update(dbArtist);

                await _context.SaveChangesAsync(cancellationToken);

                return dbArtist.SpotifyId;
            }
        }


        async Task<Result<string>> AddMissingReleases(string spotifyArtistId)
        {
            return await Result.Success()
                .Bind(GetMissingSpotifyReleases)
                .Bind(AddMissingSpotifyArtists)
                .Bind(AddMissingSpotifyReleases)
                .Map(() => spotifyArtistId);


            async Task<Result<List<SpotifyDataExtractor.Models.Releases.Release>>> GetMissingSpotifyReleases()
            {
                return await Result.Success()
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


                async Task<Result<List<SpotifyDataExtractor.Models.Releases.Release>>> GetSpotifyReleases(List<string> spotifyReleaseIds) 
                    // TODO: check for failures and replace the error message
                    => await _spotifyReleaseService.Get(spotifyReleaseIds, cancellationToken);
            }


            async Task<Result<(List<ArtistIdContainer>, List<SpotifyDataExtractor.Models.Releases.Release>)>> AddMissingSpotifyArtists(List<SpotifyDataExtractor.Models.Releases.Release> spotifyReleases)
            {
                return await Result.Success()
                    .Bind(GetFeaturingArtistIds)
                    .Bind(GetPresentFeaturingArtists)
                    .Bind(GetMissingFeaturingArtistIds)
                    .Bind(GetMissingFeaturingArtists)
                    .Bind(AddMissingFeaturingArtists);


                Result<List<string>> GetFeaturingArtistIds()
                {
                    var spotifyArtistIds = new ConcurrentBag<string>();
                    Parallel.ForEach(spotifyReleases, release =>
                    {
                        foreach (var artist in release.Artists)
                            spotifyArtistIds.Add(artist.Id);

                        foreach (var track in release.Tracks.Items)
                        {
                            foreach (var artist in track.Artists)
                                spotifyArtistIds.Add(artist.Id);
                        }
                    });

                    return spotifyArtistIds.Distinct().ToList();
                }


                async Task<Result<(List<ArtistIdContainer>, List<string>)>> GetPresentFeaturingArtists(List<string> spotifyArtistIds)
                {
                    if (spotifyArtistIds.Count == 0)
                        return (Enumerable.Empty<ArtistIdContainer>().ToList(), spotifyArtistIds);

                    var presentSpotifyArtistContainers = await _context.Artists
                        .Where(x => spotifyArtistIds.Contains(x.SpotifyId))
                        .Select(x => new ArtistIdContainer{
                                Id = x.Id, 
                                SpotifyId = x.SpotifyId 
                            })
                        .ToListAsync(cancellationToken);

                    return (presentSpotifyArtistContainers, spotifyArtistIds);
                }


                async Task<Result<(List<ArtistIdContainer>, List<string>)>> GetMissingFeaturingArtistIds((List<ArtistIdContainer> PresentArtistIdContainers, List<string> SpotifyArtistIds) container)
                {
                    if (container.SpotifyArtistIds.Count == 0)
                        return (container.PresentArtistIdContainers, Enumerable.Empty<string>().ToList());
                
                    var presentSpotifyArtistIds = await _context.Artists
                        .Where(x => container.SpotifyArtistIds.Contains(x.SpotifyId))
                        .Select(x => x.SpotifyId)
                        .ToListAsync(cancellationToken);

                    var missingFeaturingArtistIds = container.SpotifyArtistIds
                        .Except(presentSpotifyArtistIds)
                        .ToList();

                    return (container.PresentArtistIdContainers, missingFeaturingArtistIds);
                }


                async Task<Result<(List<ArtistIdContainer>, List<SpotifyDataExtractor.Models.Artists.Artist>)>> GetMissingFeaturingArtists((List<ArtistIdContainer> PresentArtistIdContainers, List<string> SpotifyArtistIds) container)
                {
                    // TODO: check for failures and replace the error message
                    var missingArtists = await _spotifyArtistService.Get(container.SpotifyArtistIds, cancellationToken);
                    return (container.PresentArtistIdContainers, missingArtists);
                }


                async Task<Result<(List<ArtistIdContainer>, List<SpotifyDataExtractor.Models.Releases.Release>)>> AddMissingFeaturingArtists((List<ArtistIdContainer> PresentArtistIdContainers, List<SpotifyDataExtractor.Models.Artists.Artist> SpotifyArtists) container)
                {
                    var dbArtists = new ConcurrentBag<Data.Artists.Artist>();
                    Parallel.ForEach(container.SpotifyArtists, artist =>
                    {
                        var dbArtist = artist.ToDbArtist(managerContext.LabelId, now);
                        dbArtists.Add(dbArtist);
                    });

                    await _context.Artists.AddRangeAsync(dbArtists, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    var addedIdContainers = new List<ArtistIdContainer>(dbArtists.Count);
                    foreach (var artist in dbArtists)
                    {
                        addedIdContainers.Add(new ArtistIdContainer
                        {
                            Id = artist.Id,
                            SpotifyId = artist.SpotifyId
                        });
                    }

                    container.PresentArtistIdContainers.AddRange(addedIdContainers);

                    return (container.PresentArtistIdContainers.ToList(), spotifyReleases);
                }
            }


            // TODO: move to ReleaseService
            async Task<Result> AddMissingSpotifyReleases((List<ArtistIdContainer> ArtistIdContainers, List<SpotifyDataExtractor.Models.Releases.Release> Releases) container)
            {
                var featuringArtistDict = container.ArtistIdContainers.ToDictionary(x => x.SpotifyId, x => x.Id);

                var dbReleases = new ConcurrentBag<Data.Releases.Release>();
                Parallel.ForEach(container.Releases, release =>
                {
                    var releaseArtists = new List<Data.Artists.Artist>(release.Artists.Count);
                    foreach (var artist in release.Artists)
                    {
                        if (!featuringArtistDict.TryGetValue(artist.Id, out var artistId))
                        {
                            // TODO: add efficient logging
                            _logger.LogWarning($"An artist with a Spotify ID '{artist.Id}' wasn't found.");
                            continue;
                        }

                        releaseArtists.Add(new Data.Artists.Artist { Id = artistId });
                    }

                    var dbRelease = release.ToDbRelease(releaseArtists, now);

                    // TODO: add release date converter
                    // TODO: add tracks

                    dbReleases.Add(dbRelease);
                });

                await _context.Releases.AddRangeAsync(dbReleases, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }


        async Task<Result<Artist>> GetArtist(string spotifyArtistId)
        {
            return await Task.FromResult(Result.Success(new Artist()));
        }
    }


    public async Task<Artist> Get(int artistId, CancellationToken cancellationToken = default)
        => (await _context.Artists
            .Where(x => x.Id == artistId)
            .Select(x => DbArtistConverter.ToArtist(x))
            .FirstOrDefaultAsync(cancellationToken))!;


    public async Task<SlimArtist> GetSlim(int artistId, CancellationToken cancellationToken = default)
        => (await _context.Artists
            .Where(x => x.Id == artistId)
            .Select(x => DbArtistConverter.ToSlimArtist(x))
            .FirstOrDefaultAsync(cancellationToken))!;


    private readonly Context _context;
    private readonly ILogger<ArtistService> _logger;
    private readonly SpotifyDataExtractor.IArtistService _spotifyArtistService;
    private readonly SpotifyDataExtractor.IReleaseService _spotifyReleaseService;
}