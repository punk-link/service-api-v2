using Core.Models.Artists;

namespace Core.Services.Artists;

public interface IReleaseStatsService
{
    public Task<ReleaseStats> Get(int artistId);
}
