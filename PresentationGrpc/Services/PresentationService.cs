using Grpc.Core;

namespace PresentationGrpc.Services;

public class PresentationService : Presentation.PresentationBase
{
    public PresentationService(ILogger<PresentationService> logger, IArtistPresentationService artistPresentationService)
    {
        _artistPresentationService = artistPresentationService;
        _logger = logger;
    }


    public override Task<HealthCheckResponse> CheckHealth(HealthCheckRequest request, ServerCallContext context) 
        => Task.FromResult(new HealthCheckResponse());


    public override Task<Artist> GetArtist(ArtistRequest request, ServerCallContext context) 
        => _artistPresentationService.Get(request.Id);


    public override Task<Release> GetRelease(ReleaseRequest request, ServerCallContext context) 
        => Task.FromResult(new Release());


    private readonly IArtistPresentationService _artistPresentationService;
    private readonly ILogger<PresentationService> _logger;
}