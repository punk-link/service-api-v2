using Core.Data.Common;
using Core.Data.Releases;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Artists;

[Index(nameof(SpotifyId), IsUnique = true)]
public class Artist : Metadata
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Column("image_details", TypeName = "jsonb")]
    public string ImageDetails { get; set; } = default!;
    public int LabelId { get; set; } // `gorm:"uniqueIndex,not null"`
    public string Name { get; set; } = default!;
    public string SpotifyId { get; set; } = default!;

    public List<Release> Releases { get; set; } = default!;
}
