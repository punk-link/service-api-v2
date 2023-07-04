using Core.Data;
using Core.Models.Artists;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Artists;

public class ArtistService : IArtistService
{
    public ArtistService(Context context)
    {
        _context = context;
    }


    public async Task<SlimArtist> GetSlim(int artistId)
        => (await _context.Artists
            .Where(x => x.Id == artistId)
            .Select(x => new SlimArtist
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync())!;


    private readonly Context _context;
}