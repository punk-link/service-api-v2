using Core.Models.Artists;

namespace Core.Services.Artists;

internal class ReleaseStatsService : IReleaseStatsService
{
    public async Task<ReleaseStats> Get(int artistId)
    {
        return await Task.FromResult(new ReleaseStats
        {
            AlbumNumber = 0,
            CompilationNumber = 0,
            SingleNumber = 0
        });
    }
}
