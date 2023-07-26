using Core.Data.Artists;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Releases;

[Index(nameof(SpotifyId), IsUnique = true)]
public class Release
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Description { get; set; } = default!;
    [Column("image_details", TypeName = "jsonb")]
    public string ImageDetails { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime ReleaseDate { get; set; }
    public string SpotifyId { get; set; } = default!;
    public int TrackNumber { get; set; }
    [Column("tracks", TypeName = "jsonb")]
    public string Tracks { get; set; } = default!;
    public string Upc { get; set; } = default!;
    public DateTime Updated { get; set; }


    public List<Artist> FeaturingArtists { get; set; } = default!;
    public List<Artist> ReleaseArtists { get; set; } = default!;
}
