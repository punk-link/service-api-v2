using Core.Models.Artists;
using Core.Models.Labels;
using CSharpFunctionalExtensions;

namespace Core.Services.Artists;

public interface IArtistService
{
    public Task<Result<Artist>> Add(ManagerContext managerContext, string spotifyId, CancellationToken cancellationToken = default);
    public Task<Result<Artist>> Get(int artistId, CancellationToken cancellationToken = default);
    public Task<Result<List<Artist>>> GetByLabel(int labelId, CancellationToken cancellationToken = default);
    public Task<SlimArtist> GetSlim(int artistId, CancellationToken cancellationToken = default);
    public Task<List<ArtistSearchResult>> Search(string query, CancellationToken cancellationToken = default);
}