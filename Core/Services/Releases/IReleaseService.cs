using Core.Models.Releases;

namespace Core.Services.Releases;

public interface IReleaseService
{
    public Task<List<SlimRelease>> GetSlim(int artistId);
}
