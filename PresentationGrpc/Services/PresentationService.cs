using Grpc.Core;

namespace PresentationGrpc.Services;

public class PresentationService : Presentation.PresentationBase
{
    public PresentationService(IArtistPresentationService artistPresentationService)
    {
        _artistPresentationService = artistPresentationService;
    }


    public override Task<HealthCheckResponse> CheckHealth(HealthCheckRequest request, ServerCallContext context) 
        => Task.FromResult(new HealthCheckResponse());


    public override Task<Artist> GetArtist(ArtistRequest request, ServerCallContext context) 
        => _artistPresentationService.Get(request.Id);


    public override Task<Release> GetRelease(ReleaseRequest request, ServerCallContext context) 
        => Task.FromResult(new Release());


    private readonly IArtistPresentationService _artistPresentationService;
}