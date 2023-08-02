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
        modelBuilder.Entity<Artist>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Artist>()
            .HasIndex(x => x.LabelId)
            .IsUnique();
        modelBuilder.Entity<Artist>()
            .HasIndex(x => x.SpotifyId)
            .IsUnique();
        modelBuilder.Entity<Artist>()
            .Property(x => x.ImageDetails)
            .HasColumnType("jsonb");

        modelBuilder.Entity<PresentationConfig>()
            .HasOne(x => x.Artist)
            .WithOne(x => x.PresentationConfig)
            .HasForeignKey<PresentationConfig>(x => x.ArtistId)
            .IsRequired();

        modelBuilder.Entity<Release>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Release>()
            .HasMany(x => x.ReleaseArtists)
            .WithMany(x => x.Releases);
        modelBuilder.Entity<Release>()
            .HasIndex(x => x.SpotifyId)
            .IsUnique();
        modelBuilder.Entity<Release>()
            .HasIndex(x => x.Upc)
            .IsUnique();
        modelBuilder.Entity<Release>()
            .Property(x => x.ImageDetails)
            .HasColumnType("jsonb");

        modelBuilder.Entity<SocialNetwork>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Track>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
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
    public DbSet<Track> Tracks { get; set; }
}
