using Core.Data;
using Core.Models.Artists;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Artists;

public class SocialNetworkService : ISocialNetworkService
{
    public SocialNetworkService(IDbContextFactory<Context> contextFactory)
    {
        _context = contextFactory.CreateDbContext();
    }


    public async Task<List<SocialNetwork>> Get(int artistId)
        => await _context.SocialNetworks
            .Where(x => x.ArtistId == artistId)
            .Select(x => new SocialNetwork
            {
                Id = x.NetworkId,
                Url = x.Url,
            })
            .ToListAsync();


    private readonly Context _context;
}
