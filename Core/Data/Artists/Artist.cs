using Core.Data.Releases;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Artists;

[Index(nameof(SpotifyId), IsUnique = true)]
public class Artist
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime Created { get; set; }
    [Column("image_details", TypeName = "jsonb")]
    public string ImageDetails { get; set; } = default!;
    public int LabelId { get; set; } // `gorm:"uniqueIndex,not null"`
    public string Name { get; set; } = default!;
    public string SpotifyId { get; set; } = default!;
    public DateTime Updated { get; set; }

    public List<Release> Releases { get; set; } = default!;
}
