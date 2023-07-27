using Core.Data.Artists;
using Core.Data.Presentations;
using Core.Data.Releases;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Release>()
            .HasMany(x => x.ReleaseArtists)
            .WithMany(x => x.Releases);

        modelBuilder.Entity<Release>()
            .HasMany(x => x.FeaturingArtists)
            .WithMany(x => x.Releases);

        modelBuilder.Entity<Track>()
            .HasOne(x => x.Release)
            .WithMany(x => x.Tracks)
            .HasForeignKey(x => x.ReleaseId)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }


    public DbSet<Artist> Artists { get; set; }
    public DbSet<PresentationConfig> PresentationConfigs { get; set; }
    public DbSet<Release> Releases { get; set; }
    public DbSet<SocialNetwork> SocialNetworks { get; set; }
}
