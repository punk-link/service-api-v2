using Core.Models.Artists;

namespace Core.Services.Artists;

public interface ISocialNetworkService
{
    public Task<List<SocialNetwork>> Get(int artistId);
}
