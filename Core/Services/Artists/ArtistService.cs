using Core.Converters.Artists;
using Core.Converters.Spotify;
using Core.Data;
using Core.Data.Artists.Validators;
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
            .Bind(AddOrLinkArtist)
            .Bind(AddMissingReleases)
            .Bind(GetArtist);


        async Task<Result<Data.Artists.Artist?>> GetExistingArtist() 
            => await _context.Artists
                .FirstOrDefaultAsync(x => x.SpotifyId == spotifyId, cancellationToken);


        async Task<Result<string>> AddOrLinkArtist(Data.Artists.Artist? dbArtist)
        {
            if (dbArtist is null)
                return await AddArtist();

            var validationResult = DbArtistValidator.ValidateDoesNotBelongsToLabel(dbArtist);
            if (!validationResult.IsValid)
                return Result.Failure<string>(validationResult.ToCombinedMessage());

            return await LinkArtist(dbArtist);


            async Task<Result<string>> AddArtist()
            {
                var spotifyArtist = await _spotifyArtistService.Get(spotifyId, cancellationToken);
                if (spotifyArtist == default)
                    return Result.Failure<string>("Can't load artist metadata from Spotify.");

                var addedDbArtist = spotifyArtist.ToDbArtist(managerContext.LabelId, now);
                await _context.Artists.AddAsync(addedDbArtist, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return addedDbArtist.SpotifyId;
            }


            async Task<Result<string>> LinkArtist(Data.Artists.Artist updatedDbArtist)
            {
                updatedDbArtist.LabelId = managerContext.LabelId;
                updatedDbArtist.Updated = now;

                _context.Update(updatedDbArtist);

                await _context.SaveChangesAsync(cancellationToken);

                return updatedDbArtist.SpotifyId;
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
        {
            var artist = await GetInternal(spotifyArtistId, cancellationToken);
            if (artist != default)
                return artist;

            _logger.LogInformation(spotifyArtistId);
            return Result.Failure<Artist>("Can't add the artist.");

        }
    }


    public async Task<Maybe<Artist>> Get(int artistId, CancellationToken cancellationToken = default)
    {
        var artist = await GetInternal(artistId, cancellationToken);
        return artist == default ? Maybe<Artist>.None : artist;
    }


    public async Task<List<Artist>> GetByLabel(int labelId, CancellationToken cancellationToken = default) 
        => await QueryArtists(x => x.LabelId == labelId)
            .ToListAsync(cancellationToken);


    public async Task<Maybe<SlimArtist>> GetSlim(int artistId, CancellationToken cancellationToken = default)
    {
        var result = await _context.Artists
            .Where(x => x.Id == artistId)
            .Select(x => x.ToSlimArtist())
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? Maybe<SlimArtist>.None;
    }


    public async Task<List<ArtistSearchResult>> Search(string query, CancellationToken cancellationToken = default)
    {
        var trimmedQuery = string.IsNullOrWhiteSpace(query) ? string.Empty : query.Trim();
        if (trimmedQuery.Length < 3)
            return Enumerable.Empty<ArtistSearchResult>().ToList();

        var results = await _spotifyArtistService.Search(trimmedQuery, cancellationToken);
        return results.Select(x => new ArtistSearchResult
            {
                ImageDetails = x.ImageDetails.ToImageDetails(),
                Name = x.Name,
                SpotifyId = x.Id
            }).ToList();
    }


    private async Task<Artist> GetInternal(int artistId, CancellationToken cancellationToken)
        => await QueryArtists(x => x.Id == artistId)
            .FirstOrDefaultAsync(cancellationToken);


    private async Task<Artist> GetInternal(string spotifyArtistId, CancellationToken cancellationToken)
        => await QueryArtists(x => x.SpotifyId == spotifyArtistId)
            .FirstOrDefaultAsync(cancellationToken);


    private IQueryable<Artist> QueryArtists(Expression<Func<Data.Artists.Artist, bool>> predicate)
        => _context.Artists
            .Where(predicate)
            .Select(x => x.ToArtist());


    private readonly Context _context;
    private readonly ILogger _logger;
    private readonly IReleaseService _releaseService;
    private readonly SpotifyDataExtractor.IArtistService _spotifyArtistService;
    private readonly SpotifyDataExtractor.IReleaseService _spotifyReleaseService;
}