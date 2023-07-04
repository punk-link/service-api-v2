using Core.Data.Artists;
using Core.Data.Presentations;
using Core.Data.Releases;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    { }


    public DbSet<Artist> Artists { get; set; }
    public DbSet<PresentationConfig> PresentationConfigs { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<SocialNetwork> SocialNetworks { get; set; }
}
