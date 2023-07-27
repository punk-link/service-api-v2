using Core.Data;
using Core.Models.Releases;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Releases;

public sealed class ReleaseService : IReleaseService
{
    public ReleaseService(Context context, SpotifyDataExtractor.IReleaseService spotifyReleaseService)
    {
        _context = context;
        _spotifyReleaseService = spotifyReleaseService;
    }


    public async Task<List<SlimRelease>> GetSlim(int artistId)
    {


        return await _context.Releases
            .Where(x => x.Id == artistId)
            .Select(x => new SlimRelease())
            .ToListAsync();
    }

    private readonly Context _context;
    private readonly SpotifyDataExtractor.IReleaseService _spotifyReleaseService;
}
