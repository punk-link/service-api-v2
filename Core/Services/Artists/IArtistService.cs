using Core.Models.Artists;

namespace Core.Services.Artists;

public interface IArtistService
{
    public Task<SlimArtist> GetSlim(int artistId);
}