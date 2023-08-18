using Core.Converters.Artists;
using Core.Converters.Spotify;
using Core.Data;
using Core.Extensions;
using Core.Models.Artists;
using Core.Models.Labels;
using Core.Models.Labels.Validators;
using Core.Services.Releases;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Artist = Core.Models.Artists.Artist;

namespace Core.Services.Artists;

public sealed class ArtistService : IArtistService
{
    public ArtistService(Context context, 
        ILoggerFactory loggerFactory,
        IReleaseService releaseService,
        SpotifyDataExtractor.IArtistService spotifyArtistService, 
        SpotifyDataExtractor.IReleaseService spotifyReleaseService)
    {
        _logger = loggerFactory.CreateLogger<ArtistService>();

        _context = context;
        _releaseService = releaseService;
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


            async Task<Result<List<SpotifyDataExtractor.Models.Releases.Release>>> AddMissingSpotifyArtists(List<SpotifyDataExtractor.Models.Releases.Release> spotifyReleases)
            {
                return await Result.Success()
                    .Bind(GetFeaturingArtistIds)
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


                async Task<Result<List<string>>> GetMissingFeaturingArtistIds(List<string> spotifyArtistIds)
                {
                    if (spotifyArtistIds.Count == 0)
                        return Enumerable.Empty<string>().ToList();
                
                    var presentSpotifyArtistIds = await _context.Artists
                        .Where(x => spotifyArtistIds.Contains(x.SpotifyId))
                        .Select(x => x.SpotifyId)
                        .ToListAsync(cancellationToken);

                    return spotifyArtistIds
                        .Except(presentSpotifyArtistIds)
                        .ToList();
                }


                async Task<Result<List<SpotifyDataExtractor.Models.Artists.Artist>>> GetMissingFeaturingArtists(List<string> spotifyArtistIds)
                    // TODO: check for failures and replace the error message
                    => await _spotifyArtistService.Get(spotifyArtistIds, cancellationToken);


                async Task<Result<List<SpotifyDataExtractor.Models.Releases.Release>>> AddMissingFeaturingArtists(List<SpotifyDataExtractor.Models.Artists.Artist> spotifyArtists)
                {
                    var dbArtists = new ConcurrentBag<Data.Artists.Artist>();
                    Parallel.ForEach(spotifyArtists, artist =>
                    {
                        var dbArtist = artist.ToDbArtist(managerContext.LabelId, now);
                        dbArtists.Add(dbArtist);
                    });

                    await _context.Artists.AddRangeAsync(dbArtists, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    return spotifyReleases;
                }
            }


            async Task<Result> AddMissingSpotifyReleases(List<SpotifyDataExtractor.Models.Releases.Release> releases)
                => await _releaseService.Add(releases, now, cancellationToken);
        }


        async Task<Result<Artist>> GetArtist(string spotifyArtistId) 
            => await GetInternal(spotifyArtistId, cancellationToken);
    }


    public Task<Result<Artist>> Get(int artistId, CancellationToken cancellationToken = default)
        // TODO: add manager check
        => GetInternal(artistId, cancellationToken);


    public async Task<Result<List<Artist>>> GetByLabel(int labelId, CancellationToken cancellationToken = default) 
        => await QueryArtists(x => x.LabelId == labelId)
            .ToListAsync(cancellationToken);


    public async Task<SlimArtist> GetSlim(int artistId, CancellationToken cancellationToken = default)
        => (await _context.Artists
            .Where(x => x.Id == artistId)
            .Select(x => DbArtistConverter.ToSlimArtist(x))
            .FirstOrDefaultAsync(cancellationToken))!;


    public async Task<List<ArtistSearchResult>> Search(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            return Enumerable.Empty<ArtistSearchResult>().ToList();

        var results = await _spotifyArtistService.Search(query, cancellationToken);
        return results.Select(x => new ArtistSearchResult
            {
                ImageDetails = x.ImageDetails.ToImageDetails(),
                Name = x.Name,
                SpotifyId = x.Id
            }).ToList();
    }


    private async Task<Result<Artist>> GetInternal(int artistId, CancellationToken cancellationToken)
        => await QueryArtists(x => x.Id == artistId)
            .FirstOrDefaultAsync(cancellationToken);


    private async Task<Result<Artist>> GetInternal(string spotifyArtistId, CancellationToken cancellationToken)
        => await QueryArtists(x => x.SpotifyId == spotifyArtistId)
            .FirstOrDefaultAsync(cancellationToken);


    private IQueryable<Artist> QueryArtists(Expression<Func<Data.Artists.Artist, bool>> predicate)
        => _context.Artists
            .Where(predicate)
            .Select(x => DbArtistConverter.ToArtist(x));


    private readonly Context _context;
    private readonly ILogger _logger;
    private readonly IReleaseService _releaseService;
    private readonly SpotifyDataExtractor.IArtistService _spotifyArtistService;
    private readonly SpotifyDataExtractor.IReleaseService _spotifyReleaseService;
}