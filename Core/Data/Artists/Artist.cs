using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Artists;

[Index(nameof(SpotifyId), IsUnique = true)]
[Table("artists")]
public class Artist
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    [Column("created")]
    public DateTime Created { get; set; }
    [Column("image_details", TypeName = "jsonb")]
    public string ImageDetails { get; set; } = default!;
    [Column("label_id")]
    public int LabelId { get; set; } // `gorm:"uniqueIndex,not null"`
    [Column("name")]
    public string Name { get; set; } = default!;
    [Column("spotify_id")]
    public string SpotifyId { get; set; } = default!;
    [Column("updated")]
    public DateTime Updated { get; set; }
}
