using Core.Models.Artists;
using Core.Models.Labels;
using CSharpFunctionalExtensions;

namespace Core.Services.Artists;

public interface IArtistService
{
    public Task<Result<Artist>> Add(ManagerContext managerContext, string spotifyId, CancellationToken cancellationToken = default);
    public Task<Result<Artist>> Get(ManagerContext managerContext, int artistId, CancellationToken cancellationToken = default);
    public Task<SlimArtist> GetSlim(ManagerContext managerContext, int artistId, CancellationToken cancellationToken = default);
}