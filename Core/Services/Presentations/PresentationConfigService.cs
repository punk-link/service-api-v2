using Core.Data;
using Core.Models.Presentations;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Presentations;

public class PresentationConfigService : IPresentationConfigService
{
    public PresentationConfigService(Context context)
    {
        _context = context;
    }


    public async Task<PresentationConfig> Get(int artistId)
    {
        var dbConfig = await _context.PresentationConfigs
            .Where(x => x.Id == artistId)
            .Select(x => new PresentationConfig())
            .FirstOrDefaultAsync();

        return dbConfig ?? Default;
    }


    private static readonly PresentationConfig Default = new()
    {
        ShareableSocialNetworkIds = Enumerable.Empty<string>().ToList()
    };


    private readonly Context _context;
}
