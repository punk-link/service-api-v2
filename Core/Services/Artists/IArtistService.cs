using Core.Models.Artists;
using Core.Models.Labels;
using CSharpFunctionalExtensions;

namespace Core.Services.Artists;

public interface IArtistService
{
    public Task<Result<Artist>> Add(ManagerContext managerContext, string spotifyId, CancellationToken cancellationToken = default);
    public Task<Maybe<Artist>> Get(int artistId, CancellationToken cancellationToken = default);
    public Task<List<Artist>> GetByLabel(int labelId, CancellationToken cancellationToken = default);
    public Task<Maybe<SlimArtist>> GetSlim(int artistId, CancellationToken cancellationToken = default);
    public Task<List<ArtistSearchResult>> Search(string query, CancellationToken cancellationToken = default);
}