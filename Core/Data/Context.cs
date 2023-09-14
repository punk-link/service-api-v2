using Core.Data.Artists;
using Core.Data.Labels;
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

        modelBuilder.Entity<Manager>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Manager>()
            .HasIndex(x => x.LabelId)
            .IsUnique();

        modelBuilder.Entity<Label>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<Label>()
            .HasMany(x => x.Managers)
            .WithOne(x => x.Label)
            .HasForeignKey(x => x.LabelId)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }


    public virtual DbSet<Artist> Artists { get; set; }
    public virtual DbSet<Label> Labels { get; set; }
    public virtual DbSet<Manager> Managers { get; set; }
    public virtual DbSet<PresentationConfig> PresentationConfigs { get; set; }
    public virtual DbSet<Release> Releases { get; set; }
    public virtual DbSet<SocialNetwork> SocialNetworks { get; set; }
    public virtual DbSet<Track> Tracks { get; set; }
}
