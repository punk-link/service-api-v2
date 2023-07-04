using Core.Services.Artists;
using Core.Services.Presentations;
using Core.Services.Releases;
using PresentationGrpc.Services.Converters;

namespace PresentationGrpc.Services;

public class ArtistPresentationService : IArtistPresentationService
{
    public ArtistPresentationService(IArtistService artistService,
        IPresentationConfigService presentationConfigService,
        IReleaseService releaseService,
        IReleaseStatsService releaseStatsService, 
        ISocialNetworkService socialNetworkService
    )
    {
        _artistService = artistService;
        _releaseService = releaseService;
        _presentationConfigService = presentationConfigService;
        _releaseStatsService = releaseStatsService;
        _socialNetworkService = socialNetworkService;
    }


    public async Task<Artist> Get(int id)
    {
        var slimArtist = await _artistService.GetSlim(id);
        var presentationConfig = await _presentationConfigService.Get(id);
        var releaseStats = await _releaseStatsService.Get(id);
        var socialNetworks = await _socialNetworkService.Get(id);

        var slimReleases = new List<Core.Models.Releases.SlimRelease>(); // await _releaseService.GetSlim(id);

        return ArtistConverters.Convert(slimArtist, presentationConfig, releaseStats, socialNetworks, slimReleases);
    }


    private readonly IArtistService _artistService;
    private readonly IPresentationConfigService _presentationConfigService;
    private readonly IReleaseService _releaseService;
    private readonly IReleaseStatsService _releaseStatsService;
    private readonly ISocialNetworkService _socialNetworkService;
}
