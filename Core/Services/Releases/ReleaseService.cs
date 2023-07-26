using Core.Data;
using Core.Models.Releases;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Releases;

public class ReleaseService : IReleaseService
{
    public ReleaseService(Context context)
    {
        _context = context;
    }


    public async Task<List<SlimRelease>> GetSlim(int artistId)
    {


        return await _context.Releases
            .Where(x => x.Id == artistId)
            .Select(x => new SlimRelease())
            .ToListAsync();
    }

    private readonly Context _context;
}
